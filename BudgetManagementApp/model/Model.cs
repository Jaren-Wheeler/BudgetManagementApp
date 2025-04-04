using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using System.Windows;

namespace BudgetManagementApp
{
    internal class Model
    {
    
        // Define the path to the database file
        string dbFile = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "model", "Data.db");

        // method to create the database
        public void initializeDatabase()
        {
            string directory = Path.GetDirectoryName(dbFile); // get the directory name where the file exists

            // if the directory doesn't exist, create it
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // if the file doesn't exist, create it
            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
            }

            // use the connection with the database file
            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();

                // sql command to create the database
                string createDB = @"CREATE TABLE IF NOT EXISTS Budget (
                                    budget_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    budget_name TEXT NOT NULL,
                                    net_income MONEY NOT NULL
                                    );

                                    CREATE TABLE IF NOT EXISTS Category (
                                    cat_id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    cat_name TEXT NOT NULL,
                                    price REAL NOT NULL,
                                    budget_id INTEGER,
                                    FOREIGN KEY (budget_id) REFERENCES Budget(budget_id)
                                    );";

                // execute the above string
                using (var command = new SQLiteCommand(createDB, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        //method to insert the budgets
        public bool insertBudget(string budgetName)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();

                string rowCount = "SELECT COUNT(*) FROM Budget WHERE budget_name = @budgetName"; // counts rows matching query to see if budget name exists
                using (var command = new SQLiteCommand(rowCount, connection))
                {
                    // add values. Intially give a net income of zero before user types it in
                    command.Parameters.AddWithValue("@budgetName", budgetName);
                    command.Parameters.AddWithValue("@net_income", 0);
                    long count = (long)command.ExecuteScalar(); // number of rows returned

                    // if no matching row with that budget name exists, insert, otherwise return false as there is already a budget with that name
                    if (count == 0)
                    {
                        string insert = "INSERT INTO Budget (budget_name,net_income) VALUES (@budgetName,@net_income)"; // sql command to insert into budget table.
                        using (var insCommand = new SQLiteCommand(insert, connection))
                        {
                            insCommand.Parameters.AddWithValue("@budgetName", budgetName); // add budget name from the inputted budget name parameter
                            insCommand.Parameters.AddWithValue("@net_income", 0); // add default value of zero for the net income
                            insCommand.ExecuteNonQuery();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // method to retrieve existing budget
        public string retrieveBudget(string input)
        {
            string budgetName = null; // initialize budget name

            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();
                string rowCount = "SELECT COUNT(*) FROM Budget WHERE budget_name = @input"; // counts the number of rows matching the budget_name

                using (var command = new SQLiteCommand(rowCount, connection))
                {
                    command.Parameters.AddWithValue("@input", input);
                    long count = (long)command.ExecuteScalar(); // returns count of rows 

                    // checks if count is > 0 (i.e. that the budget exists). If yes, it queries for the budgetName, otherwise it returns the budget name as null
                    if (count > 0)
                    {
                        string retrieve = "SELECT budget_name FROM Budget WHERE budget_name = @input"; // query for the budget name
                        using (var retrieveCommand = new SQLiteCommand(retrieve, connection))
                        {
                            retrieveCommand.Parameters.AddWithValue("@input", input);
                            budgetName = (string)retrieveCommand.ExecuteScalar(); // retrieve a single budget from database and store as a string
                        }
                        return budgetName;
                    }
                    else
                    {
                        return budgetName;
                    }

                }
            }

        }

        // retrieve the id of a budget
        public int retrieveBudgetID(string input)
        {
            int budgetID = 0; // initialize budget name

            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();
                string rowCount = "SELECT COUNT(*) FROM Budget WHERE budget_name = @input"; // counts the number of rows matching the budget id

                using (var command = new SQLiteCommand(rowCount, connection))
                {
                    command.Parameters.AddWithValue("@input", input);
                    long count = (long)command.ExecuteScalar(); // returns count of rows

                    // checks if count is > 0 (i.e. that the budget exists). If yes, it queries for the budget id, otherwise it returns the budget id as 0
                    if (count > 0)
                    {
                        string retrieve = "SELECT budget_id FROM Budget WHERE budget_name = @input"; // query for the budget id
                        using (var retrieveCommand = new SQLiteCommand(retrieve, connection))
                        {
                            retrieveCommand.Parameters.AddWithValue("@input",input);
                            var result = retrieveCommand.ExecuteScalar(); // retrieve a single budget from database and store as a string
                                            
                            // checks if the database value is null. If not, execute
                            if (result != DBNull.Value)
                            {
                                if (result is int) // Check if the result is already an int
                                {
                                    budgetID = (int)result;
                                }
                                else if (result is long) // If it's a long, cast to int
                                {
                                    budgetID = Convert.ToInt32(result);
                                }
                            }
                        }
                        return budgetID;
                    }
                    else
                    {
                        return budgetID;
                    }

                }
            }

        }

        // input budget info to the database
        public bool inputInfo(decimal netIncomeInput, string budgetName)
        {

            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();
                string rowCount = "SELECT COUNT(*) FROM Budget WHERE budget_name = @budgetName"; // counts the number of rows matching the budget_name

                using (var command = new SQLiteCommand(rowCount, connection))
                {
                    command.Parameters.AddWithValue("@budgetName", budgetName);
                    long count = (long)command.ExecuteScalar(); // returns count of rows that match the retrieve query

                    // checks if count is > 0 (i.e. that the budget exists). If yes, it queries for the budgetName, otherwise it returns the budget name as null
                    if (count > 0)
                    {
                        // perform a transaction
                        using (var transaction = connection.BeginTransaction())
                        {
                            string updateIncome = "UPDATE Budget SET net_income = @netIncomeInput WHERE budget_name = @budgetName"; // query for updating income amount
                            using (var retrieveCommand = new SQLiteCommand(updateIncome, connection))
                            {
                                retrieveCommand.Parameters.AddWithValue("@budgetName", budgetName);
                                retrieveCommand.Parameters.AddWithValue("@netIncomeInput", netIncomeInput);
                                retrieveCommand.ExecuteNonQuery();

                            }
                            transaction.Commit(); // commit the transaction.
                            return true;
                        }
                         
                    }
                    else
                    {
                        return false;
                    }

                }
            }
        }

        // retrieve the amount of the budget froom the budgets table
        public decimal retrieveBudgetAmount(string budgetName)
        {
           
            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {

                connection.Open();

                decimal budget = 0;
                string query = "SELECT net_income FROM Budget WHERE budget_name = @budgetName"; // query

                using (var command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@budgetName", budgetName);
                    var result = command.ExecuteScalar();

                    if (result != null)
                    {
                        budget = Convert.ToDecimal(result); // convert the query result to decimal if it is not null
                    }
                }

                return budget;
            }
        }

        // method to create a category
        public bool createCategory(string categoryNameInput, decimal categoryAmount, int budgetID)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();
                string rowCount = "SELECT COUNT(*) FROM Category WHERE cat_name = @categoryName AND budget_id = @budgetID"; // query to check if category exists already
                using (var rowCountCmd = new SQLiteCommand(rowCount, connection))
                {
                    rowCountCmd.Parameters.AddWithValue("@categoryName", categoryNameInput);
                    rowCountCmd.Parameters.AddWithValue("@budgetID", budgetID);
                    long count = (long)rowCountCmd.ExecuteScalar(); // number of rows matching the category name

                    if (count == 0)
                    {
                        string insertCategory = "INSERT INTO Category (cat_name,price,budget_id) VALUES (@categoryName,@price, @budgetID)"; // query for inserting a category
                        using (var insertCommand = new SQLiteCommand(insertCategory, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@categoryName", categoryNameInput);
                            insertCommand.Parameters.AddWithValue("@price", categoryAmount);
                            insertCommand.Parameters.AddWithValue("@budgetID", budgetID);
                            insertCommand.ExecuteNonQuery();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // retrieve info about a category from the database into a dictionary object
        public Dictionary<string, decimal> retrieveCategory(string categoryNameInput, int budgetID)
        {
            Dictionary<string, decimal> categories = new Dictionary<string, decimal>();

            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();

                // Query to retrieve all categories for a specific budget
                string query = "SELECT cat_name, price FROM Category WHERE budget_id = @budgetID";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    // Bind the parameter for budgetID
                    cmd.Parameters.AddWithValue("@budgetID", budgetID);

                    // Execute the query and use a reader to fetch the data
                    using (var reader = cmd.ExecuteReader())
                    {
                        // Iterate over the results
                        while (reader.Read())
                        {
                            string categoryName = reader["cat_name"].ToString(); // Retrieve the category name
                            decimal categoryPrice = reader.IsDBNull(reader.GetOrdinal("price")) ? 0 : reader.GetDecimal(reader.GetOrdinal("price")); // Retrieve the category price

                            // Add to the dictionary if it doesn't exist, otherwise set to null
                            if (!categories.ContainsKey(categoryName))
                            {
                                categories.Add(categoryName, categoryPrice);
                            }
                            else
                            {
                                categories = null;
                            }
                            
                        }
                    }
                }
            }
            return categories;
        }

        // delete category from database when the user clicks the delete button
        public bool deleteCategory(string categoryNameInput)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();
                string query = "DELETE From Category WHERE cat_name = @categoryName";

                using (var cmd = new SQLiteCommand(query,connection))
                {
                    cmd.Parameters.AddWithValue("@categoryName", categoryNameInput);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        return true;
                    } 
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // method to rename a category
        public bool renameCategory(string oldNameInput, string newNameInput) {
            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();
                string query = "UPDATE Category SET cat_name = @newNameInput WHERE cat_name = @oldNameInput";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@oldNameInput", oldNameInput);
                    cmd.Parameters.AddWithValue("@newNameInput", newNameInput);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        // update the price associated with a category in the database
        public bool updateCategoryPrice(string categoryName, string priceAdd)
        {
            using (var connection = new SQLiteConnection($"Data Source={dbFile};Version=3"))
            {
                connection.Open();
                string query = "UPDATE Category SET price = price + @priceAdd WHERE cat_name = @categoryName";

                using (var cmd = new SQLiteCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@priceAdd", priceAdd);
                    cmd.Parameters.AddWithValue("@categoryName", categoryName);
                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }
    }
}
