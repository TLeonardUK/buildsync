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
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BuildSync.Core.Utils;

namespace BuildSync.Core.Tags
{
    /// <summary>
    /// </summary>
    [Serializable]
    public class Tag
    {
        /// <summary>
        /// 
        /// </summary>
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Color Color { get; set; } = Color.FromArgb(255, 210, 212, 220);

        /// <summary>
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public bool EqualTo(Tag Other)
        {
            return Id == Other.Id &&
                Color == Other.Color &&
                Name == Other.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serializer"></param>
        public void Serialize(Networking.NetMessageSerializer serializer)
        {
            {
                string Value = Name;
                serializer.Serialize(ref Value);
                Name = Value;
            }
            {
                Guid Value = Id;
                serializer.Serialize(ref Value);
                Id = Value;
            }
            if (serializer.Version > 100000655)
            {
                Color Value = Color;
                serializer.Serialize(ref Value);
                Color = Value;
            }
        }
    }
}