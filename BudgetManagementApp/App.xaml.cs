using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace BudgetManagementApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static void ApplyTheme(string themePath)
        {
            try
            {
                var dict = new ResourceDictionary
                {
                    Source = new Uri($"pack://application:,,,/BudgetManagementApp;component/{themePath}", UriKind.Absolute)
                };

                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(dict);
            }
            catch (IOException e)
            {
                MessageBox.Show($"Resource not found");
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error loading theme: " + e);
            }

        }
    }
}
