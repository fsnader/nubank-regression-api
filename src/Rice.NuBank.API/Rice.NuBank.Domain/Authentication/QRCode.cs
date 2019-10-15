using System;

namespace Rice.NuBank.Domain.Authentication
{
    public class QrCode
    {
        public string CPF { get; set; }
        
        public string Code { get; set; }
        
        public Uri Uri { get; set; }

        public QrCode()
        {
            
        }
        
        public QrCode(string code, string cpf)
        {
            CPF = cpf;
            Code = code;
            Uri = new Uri($"https://api.qrserver.com/v1/create-qr-code/?size=150x150&data={Code}");
        }
    }
}