using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ETS_TOOL
{
    /// <summary>
    /// Interaction logic for UpdateMoneyModal.xaml
    /// </summary>
    public partial class UpdateMoneyModal : Window
    {
        public int new_money { get; set; }

        public UpdateMoneyModal()
        {
            InitializeComponent();
        }

        private void UpdateMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            if (NewMoneyTextBox.Text == string.Empty)
            {
                MessageBox.Show("Should write a name first");
                return;
            }

            new_money = int.Parse(NewMoneyTextBox.Text);
            this.DialogResult = true;
        }
    }
}
