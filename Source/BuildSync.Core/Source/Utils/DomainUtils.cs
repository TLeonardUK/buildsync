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

using System;
using System.Management;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;

namespace BuildSync.Core.Utils
{

    /// <summary>
    /// 
    /// </summary>
    public static class DomainUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDomainUsers()
        {
            List<string> Result = new List<string>();

            bool isInDomain = true;

            string domainName = Environment.UserDomainName;
            try
            {
                /*domainName = */Domain.GetComputerDomain();
            }
            catch (Exception ex)
            {
                // We are not part of a domain :|
                isInDomain = false;
            }

            if (isInDomain)
            {
                using (var context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll())
                        {
                            DirectoryEntry de = result.GetUnderlyingObject() as DirectoryEntry;
                            Result.Add(domainName + @"\" + de.Properties["samAccountName"].Value.ToString());
                        }
                    }
                }
            }
            else
            {
                var path = string.Format("WinNT://{0},computer", domainName);

                using (var computerEntry = new DirectoryEntry(path))
                {
                    foreach (DirectoryEntry childEntry in computerEntry.Children)
                    {
                        if (childEntry.SchemaClassName == "User")
                        {
                            Result.Add(domainName + @"\" + childEntry.Name);
                        }
                    }
                }
            }

            return Result;
        }
    }
}
