using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;

namespace BudgetManagementApp
{
    public partial class Homepage : UserControl
    {
        public SeriesCollection MonthlyRates { get; set; }
        private Dictionary<string, decimal> categoryValues;

        public Homepage()
        {
            InitializeComponent();
            LoadCategories();
            UpdateBudgetDisplay();
        }

        public void LoadCategories()
        {
            Model db = new Model();
    
            string budgetName = Application.Current.Properties["LoggedInBudget"] as string; // get budget name
            int budgetID = db.retrieveBudgetID(budgetName); // get budget ID

            // Retrieve updated categories from the database
            categoryValues = db.retrieveCategory(budgetName, budgetID); // Pass both budgetName and budgetID

            stckExistingCategoryNav.Children.Clear();
            MonthlyRates = new SeriesCollection();
            MonthlyRatesChart.Series = MonthlyRates;

            // for each category that exists in the database matching the current budget, add a button and a piece of the pie chart
            foreach (var category in categoryValues)
            {
                AddCategoryToChart(category.Key, category.Value);
                AddCategoryButton(category.Key);
            }
        }

        // Add a new piece of the pie chart
        private void AddCategoryToChart(string name, decimal value)
        {
            MonthlyRates.Add(new PieSeries
            {
                Title = name,
                Values = new ChartValues<decimal> { value },
                DataLabels = true
            });
        }

        // Create category button and add it to the stack panel on the homepage
        private void AddCategoryButton(string categoryName)
        {
            Button categoryBtn = new Button
            {
                Content = categoryName,
                Background = System.Windows.Media.Brushes.Blue,
                FontSize = 20,
                Foreground = System.Windows.Media.Brushes.White,
                Margin = new Thickness(5),
                Tag = categoryName
            };

            categoryBtn.Click += CategoryBtn_Click;

            Border border = new Border
            {
                Width = 300,
                BorderBrush = System.Windows.Media.Brushes.Black,
                BorderThickness = new Thickness(2),
                CornerRadius = new CornerRadius(10),
                Margin = new Thickness(5),
                Child = categoryBtn
            };

            stckExistingCategoryNav.Children.Add(border); // ad to stack panel
        }

        // click handler for clicking an category button on the homepage
        private void CategoryBtn_Click(object sender, RoutedEventArgs e)
        {
            Model db = new Model();

            // check if the user clicks an existing expense button
            if (sender is Button button && button.Tag is string categoryName)
            {
                // --- Update Amount --- //
                // Open the window which prompts the user to enter an amount to adjust the category by
                string input = Microsoft.VisualBasic.Interaction.InputBox($"Enter amount to add/subtract for '{categoryName}':",
                                                                            "Adjust Category Amount","0");

                // if the amount isn't blank and it's a valid decimal, update the database
                if (!string.IsNullOrWhiteSpace(input) && decimal.TryParse(input, out decimal adjustment))
                {
                    db.updateCategoryPrice(categoryName,input);
                }

                // --- Rename Expense Category --- //
                var renameResult = MessageBox.Show($"Do you want to rename '{categoryName}'?","Rename Category",
                                                    MessageBoxButton.YesNo,MessageBoxImage.Question);

                // if the user chooses yes in the renaming message box, execute functionality
                if (renameResult == MessageBoxResult.Yes)
                {
                    string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter new category name:", "Rename Category", categoryName);

                    // if the input for the name is valid, update the database if it exists, otherwise throw an error
                    if (!string.IsNullOrWhiteSpace(newName) && newName != categoryName)
                    {
                        if (categoryValues.ContainsKey(newName))
                        {
                            MessageBox.Show("A category with that name already exists.", "Rename Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            db.renameCategory(categoryName,newName);
                        }
                    }
                }

                // --- Delete Expense Category --- //
                var deleteResult = MessageBox.Show($"Do you want to delete '{categoryName}'?","Delete Category",MessageBoxButton.YesNo,MessageBoxImage.Warning);

                // if the user clicks yes, remove the category from the database
                if (deleteResult == MessageBoxResult.Yes)
                {
                    db.deleteCategory(categoryName);
                }

                // update the dashboard with any changes made
                LoadCategories();
                UpdateBudgetDisplay();
            }
        }

        // update the display of the budget numbers at the top of the dashboard
        public void UpdateBudgetDisplay()
        {
            Model db = new Model();
            string budgetName = Application.Current.Properties["LoggedInBudget"] as string;
            decimal monthlyBudget = db.retrieveBudgetAmount(budgetName); // the amount for net income for that budget

            decimal totalSpent = 0;

            // for each dollar amount in a category, sum them up to get the total amount spent
            foreach (var amount in categoryValues.Values)
            {
                totalSpent += amount;
            }

            decimal remaining = (decimal)monthlyBudget - totalSpent; // calculate the remaining amount by subtracting spending

            // write the updated values to the dashboard.
            lblTotalAmount.Content = $"${monthlyBudget:F2}";
            lblAmountSpent.Content = $"${totalSpent:F2}";
            lblAmountRemaining.Content = $"${remaining:F2}";
        }
    }
}
