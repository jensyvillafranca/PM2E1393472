namespace PM2E1393472
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Vistas.PaginaInicial());
        }
    }
}
