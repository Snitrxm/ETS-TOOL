using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ETS_TOOL.Classes
{
    public class Profile
    {
        private string[] _profileDataFile;
        private string _profileDataPath;
        private string _rootProfileDataPath = string.Empty;
        private Dictionary<string, int> profileInfoDict = new();
        public Dictionary<string, string> profile = new();


        public Profile(string[] profileDataFile, string profileDataPath)
        {
            _profileDataFile = profileDataFile;

            _rootProfileDataPath = profileDataPath;

            _profileDataPath = profileDataPath + @"\profile.sii";

            string[] pathParts = profileDataPath.Split(new char[] { '\\' });

            profile.Add("id", pathParts[pathParts.Length - 1]);
            for (int line = 0; line <= _profileDataFile.Length - 1; line++)
            {
                
                string currentLine = _profileDataFile[line];
                if (currentLine.Contains("profile_name"))
                {   
                    profileInfoDict.Add("profile_name_line", line);
                    string profile_name = currentLine.Split(new char[] { ':' })[1].TrimEnd().TrimStart();
                    profile.Add("name", profile_name.Replace("\"", ""));
                }
            }
        }

        public void UpdateName(string new_name)
        {
           
            byte[] newNameByte = Encoding.Default.GetBytes(new_name);
            string hexNewName = Convert.ToHexString(newNameByte);
            try
            {
                string basePath = Path.GetDirectoryName(_rootProfileDataPath);
                string newFolderPath = Path.Combine(basePath, hexNewName);;
                if(new_name.Contains(' '))
                {
                    _profileDataFile[profileInfoDict["profile_name_line"]] = String.Format("profile_name: \"{0}\"", new_name);
                } else
                {
                    _profileDataFile[profileInfoDict["profile_name_line"]] = String.Format("profile_name: {0}", new_name);
                }

                
                File.WriteAllText(_rootProfileDataPath + @"\profile.sii", string.Join(Environment.NewLine, _profileDataFile));
                Directory.Move(_rootProfileDataPath, newFolderPath);
                Directory.Delete(_rootProfileDataPath);
                MessageBox.Show("Profile Name updated successfully!");
            }
            catch (IOException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public Dictionary<string, string> GetProfile() {
            return profile;
        }
    }
}
