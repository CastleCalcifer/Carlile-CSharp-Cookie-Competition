using Microsoft.AspNetCore.DataProtection;

namespace Carlile_Cookie_Competition.Tests.Infrastructure
{
    public interface IDataProtectionService
    {
        string Protect(string data);
        string Unprotect(string protectedData);
    }

    public class DataProtectionService : IDataProtectionService
    {
        private readonly IDataProtector _protector;

        public DataProtectionService(IDataProtector protector)
        {
            _protector = protector;
        }

        public string Protect(string data)
        {
            return _protector.Protect(data);
        }

        public string Unprotect(string protectedData)
        {
            return _protector.Unprotect(protectedData);
        }
    }
}
