using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NubankClient;
using NubankClient.Model.Events;
using Rice.NuBank.Domain;
using Rice.NuBank.Domain.Authentication;
using Rice.NuBank.Domain.Metrics;
using Rice.NuBank.Domain.Utils;
using Rice.SDK.Authentication;
using Rice.SDK.Exceptions.Api;
using Rice.SDK.Utils;

namespace Rice.NuBank.Business.NuBank
{
    public class NubankService : INubankService
    {
        private readonly IConfiguration _configuration;
        private readonly ExpirableConcurrentDictionary<string, Nubank> _nubankInstances;

        public NubankService(IConfiguration configuration)
        {
            _configuration = configuration;
            _nubankInstances = new ExpirableConcurrentDictionary<string, Nubank>(TimeSpan.FromMinutes(30));
        }

        public async Task<AuthenticationToken> Login(LoginCredentials login)
        {
            if (!_nubankInstances.TryGetValue(login.CPF, out _))
            {
                var nubankClient = new Nubank(login.CPF, login.Password);
                await LoginOrThrow(nubankClient);
                
                if (_nubankInstances.TryAddValue(login.CPF, nubankClient))
                {
                    return GetToken(login.CPF, "f1a21f6b-21ce-4e42-9bd6-1aabf4a7c2bd");
                }
            }

            throw new BadRequestException();
        }
        
        private async Task LoginOrThrow(Nubank nubankClient)
        {
            try
            {
                await nubankClient.LoginAsync();
            }
            catch (Exception e)
            {
                throw new UnauthorizedException();
            }
        }

        public Task ValidateQrCode(ClaimsPrincipal user)
        {
            var cpf = user.GetUserName();
            var qrCode = user.GetQrCode();

            if (_nubankInstances.TryGetValue(cpf, out var nubankClient))
            {
                return nubankClient.AutenticateWithQrCodeAsync(qrCode);
            }

            throw new UnauthorizedException();
        }

        public async Task<string> GetQrCode(ClaimsPrincipal user)
        {
            return $"https://api.qrserver.com/v1/create-qr-code/?size=150x150&data={user.GetQrCode()}";
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

        private AuthenticationToken GetToken(string cpf, string qrCode)
        {
            int expirationMinutes = int.Parse(_configuration["Jwt:ExpirationMinutes"]);

            var token =
                new TokenBuilder(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Key"])
                    .WithUsername(cpf)
                    .WithClaims(new List<Claim>
                    {
                        new Claim("qr-code", qrCode)
                    })
                    .WithExpirationMinutes(expirationMinutes)
                    .Build();

            return new AuthenticationToken
            {
                Cpf = cpf,
                QrCode = qrCode,
                Token = token,
                Created = DateTime.Now,
                Expiration = DateTime.Now.AddMinutes(expirationMinutes),
            };
        }
    }
}