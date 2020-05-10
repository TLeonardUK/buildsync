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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using BuildSync.Core.Utils;
using BuildSync.Core.Tags;

namespace BuildSync.Core.Routes
{
    /// </summary>
    public delegate void RoutesUpdatedEventHandler();

    /// </summary>
    public delegate void RoutesDeletedEventHandler(Guid TagId);

    /// <summary>
    /// </summary>
    public class RouteRegistry
    {
        /// <summary>
        /// </summary>
        public List<Route> Routes = new List<Route>();

        /// <summary>
        /// 
        /// </summary>
        public TagRegistry TagRegistry = null;

        /// <summary>
        /// </summary>
        public event RoutesUpdatedEventHandler RoutesUpdated;

        /// <summary>
        /// </summary>
        public event RoutesDeletedEventHandler RouteDeleted;

        /// <summary>
        /// 
        /// </summary>
        public RouteRegistry(List<Route> InRoutes = null, TagRegistry InTagRegistry = null)
        {
            TagRegistry = InTagRegistry;
            TagRegistry.TagDeleted += (Guid Id) =>
            {
                bool TagsRemoved = false;

                for (int i = 0; i < Routes.Count; i++)
                {
                    Route route = Routes[i];
                    if (route.SourceTagId == Id || route.DestinationTagId == Id)
                    {
                        RouteDeleted?.Invoke(route.Id);

                        Routes.RemoveAt(i);
                        i--;

                        TagsRemoved = true;
                    }
                }

                if (TagsRemoved)
                {
                    RoutesUpdated?.Invoke();
                }
            };

            if (InRoutes != null)
            {
                Routes = InRoutes;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TagId"></param>
        public Route GetRoute(Guid SourceTagId, Guid DestinationTagId)
        {
            foreach (Route tag in Routes)
            {
                if (tag.SourceTagId == SourceTagId && tag.DestinationTagId == DestinationTagId)
                {
                    return tag;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TagId"></param>
        public Route GetRouteById(Guid TagId)
        {
            foreach (Route tag in Routes)
            {
                if (tag.Id == TagId)
                {
                    return tag;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TagId"></param>
        public void DeleteRoute(Guid TagId)
        {
            Route Tag = GetRouteById(TagId);
            if (Tag == null)
            {
                return;
            }

            Routes.Remove(Tag);

            RouteDeleted?.Invoke(TagId);

            RoutesUpdated?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SourceTags"></param>
        /// <returns></returns>
        public void GetDestinationTags(List<Guid> SourceTags, ref List<Guid> Whitelist, ref List<Guid> Blacklist)
        {
            foreach (Route tag in Routes)
            {
                if (SourceTags.Contains(tag.SourceTagId))
                {
                    if (tag.Blacklisted)
                    {
                        Blacklist.Add(tag.DestinationTagId);
                    }
                    else
                    {
                        Whitelist.Add(tag.DestinationTagId);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public void UpdateRoute(Guid RouteId, Guid SourceTagId, Guid DestinationTagId, bool Blacklisted, long BandwidthLimit)
        {
            Route Tag = GetRouteById(RouteId);
            if (Tag == null)
            {
                return;
            }

            Tag.SourceTagId = SourceTagId;
            Tag.DestinationTagId = DestinationTagId;
            Tag.Blacklisted = Blacklisted;
            Tag.BandwidthLimit = BandwidthLimit;

            RoutesUpdated?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public Guid CreateRoute(Guid SourceTagId, Guid DestinationTagId, bool Blacklisted, long BandwidthLimit)
        {
            Route Tag = new Route();
            Tag.Id = Guid.NewGuid();
            Tag.SourceTagId = SourceTagId;
            Tag.DestinationTagId = DestinationTagId;
            Tag.Blacklisted = Blacklisted;
            Tag.BandwidthLimit = BandwidthLimit;

            Routes.Add(Tag);

            RoutesUpdated?.Invoke();

            return Tag.Id;
        }
    }
}