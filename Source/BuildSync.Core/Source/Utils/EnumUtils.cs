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

using System;
using System.Reflection;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="EnumVal"></param>
        /// <returns></returns>
        public static T GetAttributeOfType<T>(this Enum EnumVal)
            where T : Attribute
        {
            Type EnumType = EnumVal.GetType();
            MemberInfo[] MemberInfo = EnumType.GetMember(EnumVal.ToString());
            object[] Attributes = MemberInfo[0].GetCustomAttributes(typeof(T), false);
            return Attributes.Length > 0 ? (T) Attributes[0] : null;
        }
    }
}