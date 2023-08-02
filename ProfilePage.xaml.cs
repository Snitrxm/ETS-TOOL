using ETS_TOOL.Classes;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ETS_TOOL
{
    /// <summary>
    /// Interaction logic for ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        private Profile _profile;
        private Bank _bank;
        private Economy _economy;
        private Dictionary<string, string> profileInfos;
        private Dictionary<string, dynamic> bankInfos;
        private CultureInfo cultureInfo = CultureInfo.GetCultureInfo("fr-FR");

        public ProfilePage(Profile profile, Bank bank, Economy economy)
        {
            InitializeComponent();
            _profile = profile;
            _bank = bank;
            _economy = economy;
            profileInfos = _profile.GetProfile();
            bankInfos = _bank.GetBankInfo();
            ProfileNameText.Text = String.Format("Name: {0}", profileInfos["name"]);
            
            string formatedMoney = String.Format(cultureInfo, "{0:C}", int.Parse(bankInfos["money_value"]));
            MoneyText.Text = String.Format("Money: {0}",formatedMoney);
        }

        private void UpdateMoneyButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateMoneyModal modal = new();

            if (modal.ShowDialog() == true)
            {
                _bank.UpdateMoney(modal.new_money);
                string formatedMoney = String.Format(cultureInfo, "{0:C}", modal.new_money);
                MoneyText.Text = String.Format("Money: {0}", formatedMoney);
            }
        }
    }
}
