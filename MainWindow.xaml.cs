using ETS_TOOL.DecryptFiles;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ETS_TOOL.Classes;
using System.Text;

namespace ETS_TOOL
{
    public partial class MainWindow : Window
    {
        private string _profilesFolder = string.Empty;
        List<string> profileNames = new List<string>();
        List<string> profileIds = new List<string>();
        private readonly Dictionary<string, string> profiles = new();
        readonly Decrypt decrypt = new();
        private string _selectedProfileFolder = string.Empty;
        private Bank bank;
        private Profile profile;
        private Economy economy;
        private string[] dataFile;
        private bool loadedInfo = false;
        public ProfilePage profilePage;


        public MainWindow()
        {
            InitializeComponent();
            LanguagesComboBox.SelectedIndex = 0;
            SwitchLanguage("en");
        }

        private void profileEtsFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new()
            {
                IsFolderPicker = true
            };
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _profilesFolder = dialog.FileName;
            }
        }

        private void LoadEtsInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_profilesFolder == string.Empty)
            {
                MessageBox.Show("Should select folder directory first!");
                return;
            }

            if (Directory.Exists(_profilesFolder))
            {
                string[] files = Directory.GetDirectories(_profilesFolder);
                profileNames.Clear();
                profileIds.Clear();
                profiles.Clear();

                foreach (string file in files)
                {
                    string[] filesInsideFolder = Directory.GetFiles(file);
                    foreach (string fileInsideFolder in filesInsideFolder)
                    {
                        if (fileInsideFolder == file + @"\profile.sii")
                        {
                            string[] profileDataFile = decrypt.NewDecodeFile(file + @"\profile.sii");

                            for (int line = 0; line <= profileDataFile.Length - 1; line++)
                            {
                                string currentLine = profileDataFile[line];
                                if (currentLine.Contains("profile_name"))
                                {
                                    string name = currentLine.Split(new char[] { ':' })[1].TrimStart().TrimEnd().Replace("\"", "");
                                    byte[] newNameByte = Encoding.Default.GetBytes(name);
                                    string hexNewName = Convert.ToHexString(newNameByte);
                                    foreach (string path in file.Split(new char[] { '\\' })) {
                                        if (path == hexNewName) {
                                            profileNames.Add(name);
                                            profileIds.Add(hexNewName);
                                            profiles.Add(name, hexNewName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                profilesComboBox.Items.Clear();
                foreach (string name in profileNames)
                {
                    profilesComboBox.Items.Add(name);
                    profilesComboBox.SelectedIndex = 0;
                }
            }
        }

        private void HandleProfileClick(object sender, EventArgs e, string profileId)
        {
            string _gameSIIFilePath = String.Format(@"{0}\{1}\save\autosave\game.sii", _profilesFolder, profileId);
            _selectedProfileFolder = String.Format(@"{0}\{1}",_profilesFolder, profileId);
            string[] decodedProfileFile = decrypt.NewDecodeFile(_selectedProfileFolder + @"\profile.sii");
            profile = new Profile(decodedProfileFile, _selectedProfileFolder);
            dataFile = decrypt.NewDecodeFile(_gameSIIFilePath);
            if (dataFile == null)
            {
                MessageBox.Show("Should open and close ETS one time to refresh data files");
                return;
            }
            for (int line = 0; line <= dataFile.Length - 1; line++)
            {
                string currentLine = dataFile[line];
                string tagLine = "", dataLine = "";

                if (currentLine.Contains(':') && currentLine.Contains('{'))
                {
                    string[] splittedLine = currentLine.Split(new char[] { ':', '{' }, 3);

                    tagLine = splittedLine[0].Trim();
                    dataLine = splittedLine[1].Trim();
                }
                else
                {
                    continue;
                }

                switch (tagLine)
                {
                    case "":
                        {
                            break;
                        }

                    case "}":
                        {
                            break;
                        }
                    case "bank":
                        {
                            bank = new Bank(dataLine, dataFile, profileId, _profilesFolder);
                            break;
                        }
                    case "economy":
                        {
                            economy = new Economy(dataFile, profileId, _profilesFolder);
                            break;
                        }
                }
                loadedInfo = true;
            }
            profilePage = new ProfilePage(profile, bank, economy);
            Main.Content = profilePage;
        }

        private void profilesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (profilesComboBox.SelectedItem != null)
            {
                string profileNameSelected = profilesComboBox.SelectedItem.ToString();
                HandleProfileClick(sender, e, profiles[profileNameSelected]);
            }
        }

        private void editProfileNameButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateProfileNameModal modal = new();

            if (modal.ShowDialog() == true) {
                profile.UpdateName(modal.new_profile_name);
                LoadEtsInfoButton_Click(sender, e);

            }
        }

        private void CargosPageButton_Click(object sender, RoutedEventArgs e)
        {
            Main.Content = new CargosPage();
        }

        private void ProfilePageButton_Click(object sender, RoutedEventArgs e)
        {
            if(loadedInfo == false) {
                MessageBox.Show("Should load ets info first.");
                return;
            }
            profilePage = new ProfilePage(profile, bank, economy);
            Main.Content = profilePage;
        }

        private void LanguagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedLanguage = LanguagesComboBox.SelectedIndex == 0 ? "en" : "tr";
            SwitchLanguage(selectedLanguage);
            profilePage?.SwitchLanguage(selectedLanguage);

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
    }
}
