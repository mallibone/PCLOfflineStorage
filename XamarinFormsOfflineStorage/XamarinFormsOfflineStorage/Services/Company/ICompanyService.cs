using System.Collections.Generic;
using System.Threading.Tasks;

namespace XamarinFormsOfflineStorage.Services.Company
{
    public interface ICompanyService
    {
        Task<IEnumerable<Models.Company>> GetCompanies();
        Task UpdateCompanies();
    }
}