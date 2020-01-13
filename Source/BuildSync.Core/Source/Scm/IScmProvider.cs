using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuildSync.Core.Scm
{
    /// <summary>
    /// 
    /// </summary>
    public interface IScmProvider
    {
        /// <summary>
        /// 
        /// </summary>
        string Server
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string Username
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string Password
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        string Root
        {
            get;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        DateTime GetSyncTime();

        /// <summary>
        /// 
        /// </summary>
        void Poll();

        /// <summary>
        /// 
        /// </summary>
        void Terminate();
    }
}
