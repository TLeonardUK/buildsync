using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Downloads;

namespace BuildSync.Server
{
    [Serializable]
    public class User
    {
        public string Username { get; set; } = "";
    }

    [Serializable]
    public class Settings : SettingsBase
    {
        public int ServerPort { get; set; } = 12341;
        public string StoragePath { get; set; } = "";
        public int MaximumManifests { get; set; } = 500;
        public List<User> Users { get; set; } = new List<User>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Username"></param>
        /// <returns></returns>
        public User GetUser(string Username)
        {
            foreach (User user in Users)
            {
                if (user.Username.ToLower() == Username.ToLower())
                {
                    return user;
                }
            }
            return null;
        }
    }
}
