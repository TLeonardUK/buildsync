using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Deployment.WindowsInstaller;

namespace BuildSync.Core.Utils
{
    public static class InstallUtils
    {
        public static string GetMsiProperty(string msi, string name)
        {
            using (Database db = new Database(msi))
            {
                return db.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", name) as string;
            }
        }

        public static void GetMsiProperty(string msi, string name, string value)
        {
            using (Database db = new Database(msi, DatabaseOpenMode.Direct))
            {
                db.Execute("UPDATE `Property` SET `Value` = '{0}' WHERE `Property` = '{1}'", value, name);
            }
        }
    }
}
