namespace XF.DrawPolygons.POC
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainPage {BindingContext = new MainPageViewModel()};
        }
    }
}