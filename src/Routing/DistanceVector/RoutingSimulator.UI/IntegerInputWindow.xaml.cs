using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RoutingSimulator.UI
{
    /// <summary>
    /// Interaction logic for IntegerInputWindow.xaml
    /// </summary>
    public partial class IntegerInputWindow : Window
    {
        public event EventHandler<int> InputEntered;
        public IntegerInputWindow(Window owner)
        {
            this.Owner = owner;
            InitializeComponent();
            Loaded += (s, e) => MoveFocus(new TraversalRequest(FocusNavigationDirection.First));
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(inputBox.Text))
                return;
            var input = int.Parse(inputBox.Text);
            InputEntered?.Invoke(this, input);
            this.Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void inputBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private bool IsTextAllowed(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return false;
            if (inputBox.Text.Length >= 8 || !Regex.IsMatch(s, @"[0-9]+"))
                return false;
            return true;
        }
    }
}
