using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BudgetManagementApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Login()
        {
            InitializeComponent();
        }

        // event handler for creating a new budget
        private void btnNewBudget_Click(object sender, RoutedEventArgs e)
        {
            Model db = new Model();

            string newBudgetName = txtNewBudgetName.Text; // user input
            if (newBudgetName == "")
            {
                MessageBox.Show("You must enter a name for your budget.");
            }
            else if (!db.insertBudget(newBudgetName))
            {
                MessageBox.Show("A budget with that name already exists");
            }
            else
            {
                db.insertBudget(newBudgetName); // insert the budget in the database with the user's budget name
                txtNewBudgetName.Text = "";
            }
        }

        // event handler for opening an existing budget
        private void btnExistingBudget_Click(object sender, RoutedEventArgs e)
        {
            Model db = new Model();

            string existingBudgetName = txtExistingBudget.Text;

            string title = db.retrieveBudget(existingBudgetName);
            decimal amount = db.retrieveBudgetAmount(existingBudgetName);

            if (existingBudgetName == "")
            {
                MessageBox.Show("You must enter a budget name.");
            }
            else if (title == null)
            {
                MessageBox.Show("The inputted budget doesn't exist.");
            }
            else
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                Homepage homepage = new Homepage();
                mainWindow.contentControl.Content = homepage;
                mainWindow.stkNavigation.Visibility = Visibility.Visible;
                mainWindow.lblBudgetTitle.Content = txtExistingBudget.Text;
                Application.Current.Properties["LoggedInBudget"] = existingBudgetName;
                homepage.LoadCategories();
                homepage.UpdateBudgetDisplay();
            }
        }

        // returns the budget name the user typed in.
        public string GetExistingBudgetName()
        {
            return txtExistingBudget.Text;
        }
    }
}
