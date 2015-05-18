using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PCLStorage;

namespace XamarinFormsOfflineStorage.Services.Company.Impl
{
    public class CompanyService:ICompanyService
    {
        private IEnumerable<Models.Company> _companies;
        private HttpClient _httpClient;

        private const string CompaniesFolder = "Companies";
        private const string CompaniesFileName = "companies.json";

        public CompanyService()
        {
            _httpClient = new HttpClient();
        }

        public async Task UpdateCompanies()
        {
            const string uri = "http://offlinestorageserver.azurewebsites.net/api/values";
            var httpResult = await _httpClient.GetAsync(uri);
            var jsonCompanies = await httpResult.Content.ReadAsStringAsync();

            var companies = JsonConvert.DeserializeObject<IEnumerable<Models.Company>>(jsonCompanies);
            var folder = await NavigateToFolder(CompaniesFolder);

            await GetImages(companies, folder);

            IFile file = await folder.CreateFileAsync(CompaniesFileName, CreationCollisionOption.ReplaceExisting);

            await file.WriteAllTextAsync(jsonCompanies);

            _companies = companies;
        }

        private async Task GetImages(IEnumerable<Models.Company> companies, IFolder folder)
        {
            //        response.EnsureSuccessStatusCode();

            //        using (IInputStream inputStream = await response.Content.ReadAsInputStreamAsync())
            //        {
            //            bitmapImage.SetSource(inputStream.AsStreamForRead());
            //        }

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

        private static async Task<IFolder> NavigateToFolder(string targetFolder)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync(targetFolder,
                CreationCollisionOption.OpenIfExists);

            return folder;
        }

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
    }
}
