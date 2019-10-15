using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using NubankClient.Model.Events;
using Rice.NuBank.Domain.Authentication;
using Rice.NuBank.Domain.Metrics;

namespace Rice.NuBank.Business.NuBank
{
    public interface INubankService
    {
        Task<AuthenticationToken> Login(LoginCredentials login);

        Task ValidateQrCode(ClaimsPrincipal user);

        Task<string> GetQrCode(ClaimsPrincipal user);

        Task<IEnumerable<Event>> GetEvents(string cpf);

        Task<MonthlySummary> GetCurrentMonthSummary(string cpf);
    }
}