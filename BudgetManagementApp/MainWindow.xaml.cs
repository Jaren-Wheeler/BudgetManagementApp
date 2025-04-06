using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BudgetManagementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model model; // instance of the model
        public Homepage homepage;
        public Login login;
        private bool isDarkMode = false;

        // private CreateCategory _createCategory;
        public MainWindow()
        {
            InitializeComponent();
            contentControl.Content = new Login(); // set the inital view to the login page user control
            stkNavigation.Visibility = Visibility.Collapsed; // initially hide the navigation bar while on the login menu

            // create the model and run the database creation
            model = new Model();
            model.initializeDatabase();

            homepage = new Homepage();// instantiate the homepage so this instance can be called later
            string budgetName = Application.Current.Properties["LoggedInBudget"] as string;
            lblBudgetTitle.Content = budgetName; // set the title at the top of the nav
            
            homepage.LoadCategories(); // load existing categories if they exist
           
        }

      
        // handles button click event when the home button is clicked
        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = homepage; // call the homepage object defined in the constructor (the same instance)

            // run these homepage methods to keep information updated
            homepage.UpdateBudgetDisplay();
            homepage.LoadCategories();
        }

        // handles event when clicking add info button
        private void btnAddInfo_Click(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new AddInfo();
        }


        // handles event when clicking toggle dark mode button
        private void btnToggleDarkMode_Click(object sender, RoutedEventArgs e)
        {
            // initially dark mode is false, but if the button is clicked it flipst to true, changing which theme is called
            if (isDarkMode)
            {
                App.ApplyTheme("Themes/Light.xaml");
            }
            else
            {
                App.ApplyTheme("Themes/Dark.xaml");
            }
            
            isDarkMode = !isDarkMode;
        }


        // handles event when clicking settings button. More will be developed here in the future
        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Prepared For Future Implementations", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // handles event when clicking log out button
        private void btnLogOut_Click(object sender, RoutedEventArgs e)
        {
      
            // The path of the current application
            string exePath = System.Reflection.Assembly.GetEntryAssembly().Location;

            // shut down the app
            Application.Current.Shutdown();

            // Restart the app
            Process.Start(exePath);
        }

      
    }
}
