using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using BuildSync.Core.Controls.Graph;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Statistic
    {
        public string Name = @"IO\Untitled (MB/s)";
        public string MaxLabel = "128 MB/s";
        public float MaxValue = 128.0f;
        public bool DefaultShown = false;

        public GraphSeries Series = new GraphSeries();

        public static Dictionary<Type, Statistic> Instances = new Dictionary<Type, Statistic>();

        public void AddSample(float Value)
        {
            Series.MinimumInterval = 1.0f / 2.0f;
            Series.AddDataPoint(Environment.TickCount / 1000.0f, Value);
        }

        public virtual void Gather()
        {
        }

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

        public static T Get<T>()
            where T : Statistic
        {
            lock (Instances)
            {
                if (!Instances.ContainsKey(typeof(T)))
                {
                    Statistic stat = Activator.CreateInstance(typeof(T)) as Statistic;
                    Instances.Add(typeof(T), stat);
                    return (T)stat;
                }
                else
                {
                    return (T)Instances[typeof(T)];
                }
            }
        }
    }
}
