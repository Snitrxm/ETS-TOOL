using ETS_TOOL.DecryptFiles;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ETS_TOOL.Classes;

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

        public MainWindow()
        {
            InitializeComponent();
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
                                    profileNames.Add(currentLine.Split(new char[] { ':' })[1].TrimStart().TrimEnd().Replace("\"", ""));
                                    profileIds.Add(file.Split(new char[] { '\\' })[8].TrimStart().TrimEnd());
                                    profiles.Add(currentLine.Split(new char[] { ':' })[1].TrimStart().TrimEnd().Replace("\"", ""), file.Split(new char[] { '\\' })[8].TrimStart().TrimEnd());
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
            string _gameSIIFilePath = String.Format(@"C:\Program Files (x86)\Steam\userdata\297974754\227300\remote\profiles\{0}\save\autosave\game.sii", profileId);
            _selectedProfileFolder = String.Format(@"C:\Program Files (x86)\Steam\userdata\297974754\227300\remote\profiles\{0}", profileId);
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
                            bank = new Bank(dataLine, dataFile, profileId);
                            break;
                        }
                    case "economy":
                        {
                            economy = new Economy(dataFile, profileId);
                            break;
                        }
                }
                loadedInfo = true;
            }
            // MessageBox.Show(String.Format("Profile With id {0} selected", profileId));
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
    }
}
