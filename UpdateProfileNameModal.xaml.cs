using System.Windows;


namespace ETS_TOOL
{
    public partial class UpdateProfileNameModal : Window
    {
        public string new_profile_name { get; set; } = string.Empty;

        public UpdateProfileNameModal()
        {
            InitializeComponent();
        }

        private void updateNameButton_Click(object sender, RoutedEventArgs e)
        {
            if (updateNameTextBox.Text == string.Empty)
            {
                MessageBox.Show("Should write a name first");
                return;
            }

            new_profile_name = updateNameTextBox.Text;
            this.DialogResult = true;
        }
    }
}
