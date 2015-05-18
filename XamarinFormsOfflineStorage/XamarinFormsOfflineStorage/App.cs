using Xamarin.Forms;
using XamarinFormsOfflineStorage.Views;

namespace XamarinFormsOfflineStorage
{
    public class App : Application
    {

        private static Locator _locator;

        public static Locator Locator
        {
            get { return _locator ?? (_locator = new Locator()); }
        }

        public App()
        {
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
