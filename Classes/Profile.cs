using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ETS_TOOL.Classes
{
    public class Profile
    {
        private string[] _profileDataFile;
        private string _profileDataPath;
        private Dictionary<string, int> profileInfoDict = new Dictionary<string, int>();


        public Profile(string[] profileDataFile, string profileDataPath)
        {
            _profileDataFile = profileDataFile;

            _profileDataPath = profileDataPath + @"\profile.sii";
            for (int line = 0; line <= _profileDataFile.Length - 1; line++)
            {
                string currentLine = _profileDataFile[line];
                if (currentLine.Contains("profile_name"))
                {
                    profileInfoDict.Add("profile_name_line", line);
                }
            }
        }

        public void UpdateName(string new_name)
        {
            _profileDataFile[profileInfoDict["profile_name_line"]] = String.Format("profile_name: {0}", new_name);
            File.WriteAllText(_profileDataPath, string.Join(Environment.NewLine, _profileDataFile));
            MessageBox.Show("Profile Name updated successfully!");
        }
    }
}
