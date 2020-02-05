using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace HelpView.View
{
    /// <summary>
    /// Логика взаимодействия для ViewThree.xaml
    /// </summary>
    public partial class ViewThree : UserControl
    {
        public IEnumerable<string> ItemsSource => GetItemsSource();

        private IEnumerable<string> GetItemsSource()
        {
            for (int i = 0; i < 50; i++)
            {
                yield return $"Item {i}";
            }
        }

        public ViewThree()
        {
            InitializeComponent();
            DataContext = this;
        }
    }
}
