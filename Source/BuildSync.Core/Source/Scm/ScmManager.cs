using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuildSync.Core.Utils;
using BuildSync.Core.Scm.Perforce;

namespace BuildSync.Core.Scm
{
    /// <summary>
    /// 
    /// </summary>
    public enum ScmProviderType
    {
        Perforce,
        Git
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScmManager
    {
        /// <summary>
        /// 
        /// </summary>
        public List<IScmProvider> Providers = new List<IScmProvider>();

        /// <summary>
        /// 
        /// </summary>
        public ScmManager()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearProviders()
        {
            foreach (IScmProvider Provider in Providers)
            {
                Provider.Terminate();
            }
            Providers.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public void AddProvider(IScmProvider Provider)
        {
            Logger.Log(LogLevel.Info, LogCategory.Scm, "Adding scm provider: {0}", Provider.ToString());

            Providers.Add(Provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Provider"></param>
        public void RemoveProvider(IScmProvider Provider)
        {
            Logger.Log(LogLevel.Info, LogCategory.Scm, "Removing scm provider: {0}", Provider.ToString());

            Provider.Terminate();
            Providers.Remove(Provider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Location"></param>
        /// <returns></returns>
        public IScmProvider GetProvider(string Location)
        {
            string Normalized = FileUtils.NormalizePath(Location);
            foreach (IScmProvider Provider in Providers)
            {
                if (FileUtils.NormalizePath(Provider.Root) == Normalized)
                {
                    return Provider;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Poll()
        {
            foreach (IScmProvider Provider in Providers)
            {
                Provider.Poll();
            }
        }
    }
}
