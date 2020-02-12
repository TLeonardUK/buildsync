/*
  buildsync
  Copyright (C) 2020 Tim Leonard <me@timleonard.uk>

  This software is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this software.
  
  Permission is granted to anyone to use this software for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this software must not be misrepresented; you must not
     claim that you wrote the original software. If you use this software
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original software.
  3. This notice may not be removed or altered from any source distribution.
*/

using Microsoft.Deployment.WindowsInstaller;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class InstallUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msi"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetMsiProperty(string msi, string name)
        {
            using (Database db = new Database(msi))
            {
                return db.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = '{0}'", name) as string;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msi"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void GetMsiProperty(string msi, string name, string value)
        {
            using (Database db = new Database(msi, DatabaseOpenMode.Direct))
            {
                db.Execute("UPDATE `Property` SET `Value` = '{0}' WHERE `Property` = '{1}'", value, name);
            }
        }
    }
}