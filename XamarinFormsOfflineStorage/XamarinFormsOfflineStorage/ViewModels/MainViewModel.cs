using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using XamarinFormsOfflineStorage.Models;
using XamarinFormsOfflineStorage.Services.Company;

namespace XamarinFormsOfflineStorage.ViewModels
{
    public class MainViewModel:ViewModelBase
    {
        private readonly ICompanyService _companyService;
        private bool _isLoadingData;

        public MainViewModel(ICompanyService companyService)
        {
            if (companyService == null) throw new ArgumentNullException("companyService");
            _companyService = companyService;

            Companies = new ObservableCollection<Company>();
            IsLoadingData = false;
            UpdateCompaniesCommand = new RelayCommand(UpdateCompanies, () => !IsLoadingData);
        }

        public bool IsLoadingData
        {
            get { return _isLoadingData; }
            set
            {
                if (value == _isLoadingData) return;
                _isLoadingData = value;
                RaisePropertyChanged(() => IsLoadingData);
            }
        }

        public ObservableCollection<Company> Companies { get; set; }

        public ICommand UpdateCompaniesCommand { get; set; }

        public async Task Init()
        {
            await UpdateCompaniesList();
        }

        private async void UpdateCompanies()
        {
            IsLoadingData = true;

            await _companyService.UpdateCompanies();
            await UpdateCompaniesList();

            IsLoadingData = false;
        }

        private async Task UpdateCompaniesList()
        {
            var companies = await _companyService.GetCompanies();

            Companies.Clear();

            foreach (var company in companies)
            {
                Companies.Add(company);
            }
        }
    }
}
