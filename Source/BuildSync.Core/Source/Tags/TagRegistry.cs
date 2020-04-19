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

namespace BuildSync.Core.Tags
{
    /// </summary>
    public delegate void TagsUpdatedEventHandler();

    /// </summary>
    public delegate void TagsDeletedEventHandler(Guid TagId);

    /// <summary>
    /// </summary>
    public class TagRegistry
    {
        /// <summary>
        /// </summary>
        public List<Tag> Tags = new List<Tag>();

        /// <summary>
        /// </summary>
        public event TagsUpdatedEventHandler TagsUpdated;

        /// <summary>
        /// </summary>
        public event TagsDeletedEventHandler TagDeleted;

        /// <summary>
        /// 
        /// </summary>
        public TagRegistry(List<Tag> InTags = null)
        {
            if (InTags != null)
            {
                Tags = InTags;
            }

            if (Tags.Count == 0)
            {
                Tags.Add(new Tag() { Id = Guid.NewGuid(), Name = "Broken" });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public string IdToString(Guid Id)
        {
            Tag tag = GetTagById(Id);
            if (tag != null)
            {
                return tag.Name;
            }
            else
            {
                return "Unknown[" + Id + "]";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Ids"></param>
        /// <returns></returns>
        public string IdsToString(List<Guid> Ids)
        {
            string Value = "";
            foreach (Guid id in Ids)
            {
                if (Value.Length > 0)
                {
                    Value += ", ";
                }

                Tag tag = GetTagById(id);
                if (tag != null)
                {
                    Value += tag.Name;
                }
                else
                {
                    Value += "Unknown[" + id + "]";
                }
            }
            return Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="TagId"></param>
        public Tag GetTagById(Guid TagId)
        {
            foreach (Tag tag in Tags)
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
        public void DeleteTag(Guid TagId)
        {
            Tag Tag = GetTagById(TagId);
            if (Tag == null)
            {
                return;
            }

            Tags.Remove(Tag);

            TagDeleted?.Invoke(TagId);

            TagsUpdated?.Invoke();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        public Guid CreateTag(string Name)
        {
            Tag Tag = new Tag();
            Tag.Id = Guid.NewGuid();
            Tag.Name = Name;

            Tags.Add(Tag);

            TagsUpdated?.Invoke();

            return Tag.Id;
        }
    }
}