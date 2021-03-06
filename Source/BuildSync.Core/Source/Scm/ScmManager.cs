﻿/*
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

using System.Collections.Generic;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Scm
{
    /// <summary>
    /// </summary>
    public enum ScmProviderType
    {
        Perforce,
        Git
    }

    /// <summary>
    /// </summary>
    public class ScmManager
    {
        /// <summary>
        /// </summary>
        public List<IScmProvider> Providers = new List<IScmProvider>();

        /// <summary>
        /// </summary>
        public void AddProvider(IScmProvider Provider)
        {
            Logger.Log(LogLevel.Info, LogCategory.Scm, "Adding scm provider: {0}", Provider.ToString());

            Providers.Add(Provider);
        }

        /// <summary>
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
        /// </summary>
        public void Poll()
        {
            foreach (IScmProvider Provider in Providers)
            {
                Provider.Poll();
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="Provider"></param>
        public void RemoveProvider(IScmProvider Provider)
        {
            Logger.Log(LogLevel.Info, LogCategory.Scm, "Removing scm provider: {0}", Provider.ToString());

            Provider.Terminate();
            Providers.Remove(Provider);
        }
    }
}