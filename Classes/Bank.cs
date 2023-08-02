using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ETS_TOOL.Classes
{
    public class Bank
    {
        private Dictionary<string, dynamic> money_dict = new Dictionary<string, dynamic>();
        private string[] _dataFile;
        private readonly string _dataFilePath = string.Empty;

        public Bank(string bankIdentificator, string[] _file, string profileId)
        {
            _dataFile = _file;
            _dataFilePath = String.Format(@"C:\Program Files (x86)\Steam\userdata\297974754\227300\remote\profiles\{0}\save\autosave\game.sii", profileId);
            for (int line = 0; line <= _file.Length - 1; line++)
            {
                string currentLine = _file[line];
                if (currentLine.Contains(bankIdentificator) && currentLine.Contains("{"))
                {
                    //First line after bank identificator
                    string money_account = _file[line + 1];
                    string money_value = money_account.Split(new char[] { ':' })[1];
                    money_dict.Add("money_line", line + 1);
                    money_dict.Add("money_value", money_value);
                    break;
                }
            }

        }

        public void UpdateMoney(int new_money)
        {
            int money_line = money_dict["money_line"];
            _dataFile[money_line] = String.Format("money_account: {0}", new_money.ToString());
            File.WriteAllText(_dataFilePath, string.Join(Environment.NewLine, _dataFile));
            MessageBox.Show("Money updated successfully!");
        }
    }
}
