using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ETS_TOOL.Classes
{
    public class Economy
    {
        private Dictionary<string, dynamic> experience_dict = new Dictionary<string, dynamic>();
        string[] _dataFile;
        string _dataFilePath = string.Empty;

        public Economy(string[] _file, string profileId, string _profilesFolder)
        {
            _dataFilePath = String.Format($@"{_profilesFolder}\{profileId}\save\autosave\game.sii");
            _dataFile = _file;
            for (int line = 0; line <= _file.Length - 1; line++)
            {
                string currentLine = _file[line];
                if (currentLine.Contains("experience_points"))
                {
                    experience_dict.Add("experience_line", line);
                    experience_dict.Add("experience_value", currentLine.Split(new char[] { ':' })[1].TrimEnd().TrimStart()); ;
                    break;
                }
            }
        }

        public void UpdateExperience(int new_experience_value)
        {
            int experience_line = experience_dict["experience_line"];
            _dataFile[experience_line] = String.Format("experience_points: {0}", new_experience_value.ToString());
            File.WriteAllText(_dataFilePath, string.Join(Environment.NewLine, _dataFile));
            MessageBox.Show("Experience points updated successfully!");
        }

        public Dictionary<string, dynamic> getEconomyInfo()
        {
            return experience_dict;
        }
    }
}
