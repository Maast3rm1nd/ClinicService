using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicServiceBase.DAL
{
    public interface IServiceCollectionRegister
    {
        public IServiceCollection Register(IServiceCollection services, IConfiguration? config = null);
    }
}
