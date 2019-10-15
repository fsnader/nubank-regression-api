using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NubankClient;
using NubankClient.Model.Events;
using Rice.NuBank.Domain;
using Rice.NuBank.Domain.Authentication;
using Rice.NuBank.Domain.Metrics;
using Rice.SDK.Exceptions.Api;

namespace Rice.NuBank.Business.NuBank
{
    public class NubankService : INubankService
    {
        private readonly ExpirableConcurrentDictionary<string, Nubank> _nubankInstances;

        public NubankService()
        {
            _nubankInstances = new ExpirableConcurrentDictionary<string, Nubank>(TimeSpan.FromMinutes(30));
        }

        public async Task<QrCode> Login(LoginCredentials login)
        {
            if (!_nubankInstances.TryGetValue(login.CPF, out _))
            {
                var nubankClient = new Nubank(login.CPF, login.Password);
                var result = await nubankClient.LoginAsync();

                if (_nubankInstances.TryAddValue(login.CPF, nubankClient))
                {
                    return new QrCode(result.Code, login.CPF);
                }
            }

            throw new BadRequestException();
        }

        public Task ValidateQrCode(QrCode qrCode)
        {
            if (_nubankInstances.TryGetValue(qrCode.CPF, out var nubankClient))
            {
                return nubankClient.AutenticateWithQrCodeAsync(qrCode.Code);
            }

            throw new UnauthorizedException();
        }

        public Task<IEnumerable<Event>> GetEvents(string cpf)
        {
            if (_nubankInstances.TryGetValue(cpf, out var nubankClient))
            {
                return nubankClient.GetEventsAsync();
            }

            throw new UnauthorizedException();
        }

        public async Task<MonthlySummary> GetCurrentMonthSummary(string cpf)
        {
            var events = await GetEvents(cpf);

            return new MonthlySummary(
                DateTime.Now.Month,
                events.Where(e => e.Time.Year == DateTime.Now.Year && e.Time.Year == DateTime.Now.Month));
        }
    }
}