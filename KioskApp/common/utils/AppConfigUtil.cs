using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KioskApp.common.utils
{
     public class AppConfigUtil
    {
        public static string get(string key)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            string value = config.AppSettings.Settings[key].Value;
            return value;

        }
    }
}
