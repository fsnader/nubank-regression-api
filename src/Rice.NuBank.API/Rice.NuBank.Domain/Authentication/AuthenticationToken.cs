using System;

namespace Rice.NuBank.Domain.Authentication
{
    public class AuthenticationToken
    {
        public string Cpf { get; set; }
        public string QrCode { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; }
        public DateTime Expiration { get; set; }
    }
}