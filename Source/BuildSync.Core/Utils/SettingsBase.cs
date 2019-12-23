using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace BuildSync.Core.Utils
{
    /// <summary>
    /// 
    /// </summary>
    public class SettingsBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FullFilePath"></param>
        /// <returns></returns>
        public bool Save(string FullFilePath)
        {
            bool Result = false;
            try
            {
                string DirPath = Path.GetDirectoryName(FullFilePath);
                if (!Directory.Exists(DirPath))
                {
                    Directory.CreateDirectory(DirPath);
                }

                using (TextWriter TextWriter = new StreamWriter(FullFilePath))
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(GetType());

                    XmlSerializer.Serialize(TextWriter, this);
                    Result = true;
                }
            }
            catch (IOException)
            {
            }
            return Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FullFilePath"></param>
        /// <param name="SaveItem"></param>
        /// <returns></returns>
        public static bool Load<T>(string FullFilePath, out T SaveItem)
            where T : new()
        {
            bool Result = false;
            SaveItem = new T();

            try
            {
                using (TextReader TextReader = new StreamReader(FullFilePath))
                {
                    XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));
                    SaveItem = (T)XmlSerializer.Deserialize(TextReader);
                    if (SaveItem != null)
                    {
                        Result = true;
                    }
                }
            }
            catch (DirectoryNotFoundException)
            {
            }
            catch (FileNotFoundException)
            {
            }
            return Result;
        }
    }
}
