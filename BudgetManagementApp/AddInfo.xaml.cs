using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace BudgetManagementApp
{
    /// <summary>
    /// Interaction logic for AddInfo.xaml
    /// </summary>
    public partial class AddInfo : UserControl
    {
        public string name { get; set; } // name of budget
        public int budgetID { get; set; } // id of budget

        // constructor
        public AddInfo()
        {
            InitializeComponent();
            Model db = new Model();

          
            // connect button clicks
            btnEnterInfo.Click += BtnEnterInfo_Click;
            btnSave.Click += BtnSave_Click;
        }

        // button click handler for entering net income value
        private void BtnEnterInfo_Click(object sender, RoutedEventArgs e)
        {
            // if the net income value is a valid decimal, write it to the database and update the homepage
            if (decimal.TryParse(txtNetIncome.Text, out decimal monthlyBudget))
            {
                Model db = new Model();
                string budgetName = Application.Current.Properties["LoggedInBudget"] as string;
               
                db.inputInfo(monthlyBudget, budgetName); // input the value into the database

                var mainWindow = Application.Current.MainWindow as MainWindow; // get the current instance of the mainwindow

                // change the view back to the homepage
                var homepage = new Homepage();
                mainWindow.contentControl.Content = homepage;
             
            }
            else
            {
                MessageBox.Show("Please enter a valid number for net income.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // button click handler for entering expense information
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            Model db = new Model();
            string budgetName = Application.Current.Properties["LoggedInBudget"] as string; // name of the budget that is currently logged in

            name = txtCategoryName.Text.Trim(); // the name of the category
            string amount = txtAmount.Text; // amount in the category
            budgetID = db.retrieveBudgetID(budgetName); //the budget id to connect with current budget in database

            // validate inputs for expense name and amount
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a category name.");
                return;
            }

            if (!decimal.TryParse(txtAmount.Text, out decimal value))
            {
                MessageBox.Show("Please enter a valid amount.");
                return;
            }

            decimal dollarValue = decimal.Parse(amount); // parse the inputted amount into a decimal

            var categories = db.retrieveCategory(name,budgetID); // Retrieves the category from the database

            // if the categories dictionary contains the name already. If so, throw an error that it already exists. Otherwise, create it in the database
            if (categories.ContainsKey(name))
            {
                MessageBox.Show("You already have an expense called " + name);
                return;
            }
            else
            {
                var createCategory = db.createCategory(name, dollarValue, budgetID); // save categories to db
                MessageBox.Show("Category saved.");

            }
               
            //Navigate to homepage with updated data
            var homepage = new Homepage();
            homepage.LoadCategories(); // Reload categories on the homepage
            ((MainWindow)Application.Current.MainWindow).contentControl.Content = homepage; 
        }
    }
}
