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
using System.Reflection;
using BuildSync.Core.Controls.Graph;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// </summary>
    public abstract class Statistic
    {
        public static Dictionary<Type, Statistic> Instances = new Dictionary<Type, Statistic>();
        public bool DefaultShown = false;
        public string MaxLabel = "128 MB/s";
        public float MaxValue = 128.0f;
        public string Name = @"IO\Untitled (MB/s)";

        public GraphSeries Series = new GraphSeries();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Value"></param>
        public void AddSample(float Value)
        {
            Series.MinimumInterval = 1.0f / 2.0f;
            Series.AddDataPoint(Environment.TickCount / 1000.0f, Value);
        }

        /// <summary>
        /// 
        /// </summary>

        public virtual void Gather()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Get<T>()
            where T : Statistic
        {
            lock (Instances)
            {
                if (!Instances.ContainsKey(typeof(T)))
                {
                    Statistic stat = Activator.CreateInstance(typeof(T)) as Statistic;
                    Instances.Add(typeof(T), stat);
                    return (T) stat;
                }

                return (T) Instances[typeof(T)];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Instantiate()
        {
            lock (Instances)
            {
                foreach (Assembly Assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        foreach (Type Type in Assembly.GetTypes())
                        {
                            if (typeof(Statistic).IsAssignableFrom(Type))
                            {
                                if (!Instances.ContainsKey(Type) && !Type.IsAbstract)
                                {
                                    Statistic stat = Activator.CreateInstance(Type) as Statistic;
                                    Instances.Add(Type, stat);
                                }
                            }
                        }
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        // Skip this assembly, for some reason any runtime generated (via dynamic complilation) assemblies
                        // cannot have their types examined.
                    }
                }
            }
        }
    }
}