using ETS_TOOL.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

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
        private Dictionary<string, dynamic> economyInfos;
        private CultureInfo cultureInfo = CultureInfo.GetCultureInfo("fr-FR");
        private string qtdMoneyFirstTime = string.Empty;
        private string qtdExperiencePointsFirstTime = string.Empty;

        public ProfilePage(Profile profile, Bank bank, Economy economy)
        {
            InitializeComponent();
            SwitchLanguage("en");
            _profile = profile;
            _bank = bank;
            _economy = economy;
            profileInfos = _profile.GetProfile();
            bankInfos = _bank.GetBankInfo();
            economyInfos = _economy.getEconomyInfo();
            ProfileNameText.Text = String.Format("Name: {0}", profileInfos["name"]);
            SaveChangesButton.IsEnabled = false;

            //string formatedMoney = String.Format(cultureInfo, "{0:C}", int.Parse(bankInfos["money_value"]));
            //MoneyText.Text = String.Format("Money: {0}",formatedMoney);
            qtdMoneyFirstTime = bankInfos["money_value"];
            qtdExperiencePointsFirstTime = economyInfos["experience_value"];
            MoneyTextBox.Text = bankInfos["money_value"];
            ExperiencePointsTextBox.Text = economyInfos["experience_value"];
        }

        public void SwitchLanguage(string code)
        {
            ResourceDictionary dict = new();
            switch (code)
            {
                case "en":
                    dict.Source = new Uri("..\\Resources\\Languages\\en-US.xaml", UriKind.Relative);
                    break;
                case "tr":
                    dict.Source = new Uri("..\\Resources\\Languages\\tr-TR.xaml", UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
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

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (MoneyTextBox.Text != qtdMoneyFirstTime)
            {
                _bank.UpdateMoney(int.Parse(MoneyTextBox.Text));
                qtdMoneyFirstTime = MoneyTextBox.Text;
            }

            if (ExperiencePointsTextBox.Text != qtdExperiencePointsFirstTime)
            {
                _economy.UpdateExperience(int.Parse(ExperiencePointsTextBox.Text));
                qtdExperiencePointsFirstTime = ExperiencePointsTextBox.Text;
            }
           
            SaveChangesButton.IsEnabled = false;
        }

        private void MoneyTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (MoneyTextBox.Text != qtdMoneyFirstTime) {
                SaveChangesButton.IsEnabled = true;
            }
        }

        private void ExperiencePointsTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ExperiencePointsTextBox.Text != qtdExperiencePointsFirstTime)
            {
                SaveChangesButton.IsEnabled = true;
            }
        }
    }
}
