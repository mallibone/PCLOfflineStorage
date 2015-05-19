using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;

namespace XamarinFormsOfflineStorage.Services.Company.Impl
{
    public class CompanyService:ICompanyService
    {
        private IEnumerable<Models.Company> _companies;
        private readonly HttpClient _httpClient;

        private const string CompaniesFolder = "Companies";
        private const string CompaniesFileName = "companies.json";

        public CompanyService()
        {
            _httpClient = new HttpClient();
        }

        #region UpdateCompanies
        public async Task UpdateCompanies()
        {
            const string uri = "http://offlinestorageserver.azurewebsites.net/api/values";
            var httpResult = await _httpClient.GetAsync(uri);
            var jsonCompanies = await httpResult.Content.ReadAsStringAsync();

            var companies = JsonConvert.DeserializeObject<ICollection<Models.Company>>(jsonCompanies);
            var folder = await NavigateToFolder(CompaniesFolder);

            await StoreImagesLocally(folder, companies);

            await SerializeCompanies(folder, companies);

            _companies = companies;
        }

        private static async Task SerializeCompanies(IFolder folder, ICollection<Models.Company> companies)
        {
            IFile file = await folder.CreateFileAsync(CompaniesFileName, CreationCollisionOption.ReplaceExisting);
            var companiesString = JsonConvert.SerializeObject(companies);
            await file.WriteAllTextAsync(companiesString);
        }

        private async Task StoreImagesLocally(IFolder folder, IEnumerable<Models.Company> companies)
        {
            foreach (var company in companies)
            {
                var file = await folder.CreateFileAsync(company.Name + ".jpg", CreationCollisionOption.ReplaceExisting);
                using (var fileHandler = await file.OpenAsync(FileAccess.ReadAndWrite))
                {
                    var httpResponse = await _httpClient.GetAsync(company.ImageUri);
                    byte[] imageBuffer = await httpResponse.Content.ReadAsByteArrayAsync();
                    await fileHandler.WriteAsync(imageBuffer, 0, imageBuffer.Length);

                    company.ImageUri = file.Path;
                }
            }
        }
        #endregion

        #region GetCompanies
        public async Task<IEnumerable<Models.Company>> GetCompanies()
        {
            return _companies ?? (_companies = await ReadCompaniesFromFile());
        }

        private async Task<IEnumerable<Models.Company>>  ReadCompaniesFromFile()
        {
            var folder = await NavigateToFolder(CompaniesFolder);

            if ((await folder.CheckExistsAsync(CompaniesFileName)) == ExistenceCheckResult.NotFound)
            {
                return new List<Models.Company>();
            }

            IFile file = await folder.GetFileAsync(CompaniesFileName);
            var jsonCompanies = await file.ReadAllTextAsync();

            if (string.IsNullOrEmpty(jsonCompanies)) return new List<Models.Company>();

            var companies = JsonConvert.DeserializeObject<IEnumerable<Models.Company>>(jsonCompanies);

            return companies;
        }
        #endregion

        private static async Task<IFolder> NavigateToFolder(string targetFolder)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(targetFolder,
                CreationCollisionOption.OpenIfExists);

            return folder;
        }
    }
}
