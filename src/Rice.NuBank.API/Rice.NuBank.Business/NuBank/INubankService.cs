using System.Collections.Generic;
using System.Threading.Tasks;
using NubankClient.Model.Events;
using Rice.NuBank.Domain.Authentication;
using Rice.NuBank.Domain.Metrics;

namespace Rice.NuBank.Business.NuBank
{
    public interface INubankService
    {
        Task<QrCode> Login(LoginCredentials login);
        
        Task ValidateQrCode(QrCode qrCode);

        Task<IEnumerable<Event>> GetEvents(string cpf);

        Task<MonthlySummary> GetCurrentMonthSummary(string cpf);
    }
}