using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;
using Prism.Navigation;
using SuperShop.Prism.Models;
using SuperShop.Prism.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SuperShop.Prism.ViewModels
{
    public class ProductsPageViewModel : ViewModelBase
    {
        private readonly IApiService _apiService;
        private readonly INavigationService _navigationService;

        private List<ProductResponse> _listProducts;
        private ObservableCollection<ProductResponse> _productsObsCol;

        private bool _isRunning;
        private string _search;

        private DelegateCommand _searchCommand;

        public ProductsPageViewModel(IApiService apiService, INavigationService navigationService)
            : base(navigationService)
        {
            _apiService = apiService;
            _navigationService = navigationService;

            base.Title = "Products";

            LoadProductsAsync();
        }


        public ObservableCollection<ProductResponse> Products
        {
            get => _productsObsCol;
            set => SetProperty(ref _productsObsCol, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public string Search
        {
            get => _search;
            set
            {
                SetProperty(ref _search, value);
                ShowProducts();
            }
        }

        public DelegateCommand SearchCommand => _searchCommand ??
            (_searchCommand = new DelegateCommand(ShowProducts));


        private async void LoadProductsAsync()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await App.Current.MainPage.DisplayAlert(
                        "Error", "Check internet connection", "Accept");
                });
                
                return;
            }

            IsRunning = true;

            string url = App.Current.Resources["UrlApi"].ToString();

            Response getList = await _apiService.GetListAsync<ProductResponse>(url, "/api", "/products");

            IsRunning = false;

            if (!getList.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert("Error", getList.Message, "Accept");
                return;
            }

            _listProducts = (List<ProductResponse>)getList.Result;

            ShowProducts();
        }

        private void ShowProducts()
        {
            if (string.IsNullOrEmpty(Search))
            {
                Products = new ObservableCollection<ProductResponse>(_listProducts);
            }
            else
            {
                Products = new ObservableCollection<ProductResponse>(
                    _listProducts.Where(p => p.Name.ToLower().Contains(Search)));
            }
        }
    }
}
