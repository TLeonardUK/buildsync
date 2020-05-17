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
using System.Linq;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public static class ListUtils
    {
        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static IList<T> Clone<T>(this IList<T> listToClone)
            where T : ICloneable
        {
            return listToClone.Select(item => (T) item.Clone()).ToList();
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static bool ContainsAny<T>(this IList<T> haystack, IList<T> needles)
        {
            foreach (T needle in needles)
            {
                if (haystack.Contains(needle))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static bool IsEqual<T>(this IList<T> listA, IList<T> listB, bool orderIndependent = true)
        {
            if (listA.Count != listB.Count)
            {
                return false;
            }

            if (orderIndependent)
            {
                List<T> Clone = new List<T>(listA);
                foreach (T val in listB)
                {
                    Clone.Remove(val);
                }

                if (Clone.Count > 0)
                {
                    return false;
                }
            }
            else
            {
                for (int i = 0; i < listA.Count; i++)
                {
                    if (!listA[i].Equals(listB[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static List<T> GetAdded<T>(this IList<T> listA, IList<T> baseList)
        {
            List<T> Result = new List<T>();
            foreach (T val in listA)
            {
                if (!baseList.Contains(val))
                {
                    Result.Add(val);
                }
            }            
            return Result;
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static List<T> GetRemoved<T>(this IList<T> listA, IList<T> baseList)
        {
            List<T> Result = new List<T>();
            foreach (T val in baseList)
            {
                if (!listA.Contains(val))
                {
                    Result.Add(val);
                }
            }
            return Result;
        }
    }
}