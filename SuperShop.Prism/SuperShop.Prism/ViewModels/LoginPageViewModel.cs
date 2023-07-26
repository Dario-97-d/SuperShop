using Prism.Commands;
using Prism.Navigation;

namespace SuperShop.Prism.ViewModels
{
    public class LoginPageViewModel : ViewModelBase
    {
        private bool _isEnabled;
        private bool _isRunning;
        private string _password;

        private readonly DelegateCommand _loginCommand;

        public LoginPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Login";

            IsEnabled = true;
        }


        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public string Email { get; set; }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }


        public DelegateCommand LoginCommand => _loginCommand ?? new DelegateCommand(Login);


        private async void Login()
        {
            if (string.IsNullOrEmpty(Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Email is required",
                    "Accept");

                Password = string.Empty;
                return;
            }

            if (string.IsNullOrEmpty(_password))
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Password is required",
                    "Accept");

                Password = string.Empty;
                return;
            }

            // Login simulation
            await App.Current.MainPage.DisplayAlert(
                    "Login",
                    "Login successful",
                    "Accept");
            return;
        }
    }
}
