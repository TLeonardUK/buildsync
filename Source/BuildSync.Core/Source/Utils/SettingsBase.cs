using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.Text.Encodings.Web;

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
                    JsonSerializerOptions Options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    string Data = JsonSerializer.Serialize(this, GetType(), Options);
                    TextWriter.Write(Data);
                    Result = true;

                    /*
                    JsonSerializer XmlSerializer = new JsonSerializer(GetType());

                    XmlSerializer.Serialize(TextWriter, this);
                    Result = true;
                    */
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Failed to save '{0}' with error: {1}", FullFilePath, Ex.Message);
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
                JsonSerializerOptions Options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string Text = File.ReadAllText(FullFilePath);
                SaveItem = (T)JsonSerializer.Deserialize(Text, typeof(T), Options);
                if (SaveItem != null)
                {
                    Result = true;
                }

                //using (TextReader TextReader = new StreamReader(FullFilePath))
                //{
                /*
                XmlSerializer XmlSerializer = new XmlSerializer(typeof(T));
                SaveItem = (T)XmlSerializer.Deserialize(TextReader);
                if (SaveItem != null)
                {
                    Result = true;
                }*/
                //}
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Info, LogCategory.Main, "Failed to load '{0}' with error: {1}", FullFilePath, Ex.Message);
            }
            return Result;
        }
    }
}
