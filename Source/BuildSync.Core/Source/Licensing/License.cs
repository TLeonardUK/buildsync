﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using BuildSync.Core.Utils;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace BuildSync.Core.Licensing
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class License
    {
        /// <summary>
        /// 
        /// </summary>
        public static DateTime InfiniteExpirationTime = new DateTime(0);

        /// <summary>
        /// 
        /// </summary>
        public string LicensedTo = "";

        /// <summary>
        /// 
        /// </summary>
        public int MaxSeats = 0;

        /// <summary>
        /// 
        /// </summary>
        public DateTime ExpirationTime = DateTime.UtcNow;

        /// <summary>
        /// 
        /// </summary>
        public byte[] Signature = null;

        /// <summary>
        /// 
        /// </summary>
        public bool IsExpired
        {
            get
            {
                if (ExpirationTime == InfiniteExpirationTime)
                {
                    return false;
                }
                return (DateTime.Now > ExpirationTime);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string VerifyData
        {
            get
            {
                return string.Format("{0},{1},{2}", LicensedTo, MaxSeats, ExpirationTime);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Verify()
        {
            Stream CertStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BuildSync.Core.Resources.public.cer");
            if (CertStream == null)
            {
                Logger.Log(LogLevel.Error, LogCategory.Licensing, "Failed to read public certificate.");
                return false;
            }

            if (Signature == null)
            {
                Logger.Log(LogLevel.Error, LogCategory.Licensing, "License file is not signed.");
                return false;
            }

            byte[] Buffer = new byte[CertStream.Length];            
            int ReadBytes = CertStream.Read(Buffer, 0, (int)CertStream.Length);
            if (ReadBytes == 0)
            {
                Logger.Log(LogLevel.Error, LogCategory.Licensing, "Failed to read all of public certificate's bytes.");
                return false;
            }

            X509Certificate2 Certificate = new X509Certificate2(Buffer);
            RSACryptoServiceProvider ServiceProvider = (RSACryptoServiceProvider)Certificate.PublicKey.Key;

            SHA1Managed Hasher = new SHA1Managed();
            UnicodeEncoding Encoding = new UnicodeEncoding();
            byte[] Data = Encoding.GetBytes(VerifyData);
            byte[] Hash = Hasher.ComputeHash(Data);

            return ServiceProvider.VerifyHash(Hash, CryptoConfig.MapNameToOID("SHA1"), Signature);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Sign(string CertPath)
        {
            X509Certificate2 Certificate = new X509Certificate2(CertPath, "");
            RSACryptoServiceProvider ServiceProvider = (RSACryptoServiceProvider)Certificate.PrivateKey;

            SHA1Managed Hasher = new SHA1Managed();
            UnicodeEncoding Encoding = new UnicodeEncoding();
            byte[] Data = Encoding.GetBytes(VerifyData);
            byte[] Hash = Hasher.ComputeHash(Data);

            Signature = ServiceProvider.SignHash(Hash, CryptoConfig.MapNameToOID("SHA1"));
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            try
            {
                FileUtils.WriteToBinaryFile(path, this);
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Licensing, "Failed to save license file with error: {0}", Ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public static License Load(string path)
        {
            License result = null;
            try
            {
                result = FileUtils.ReadFromBinaryFile<License>(path);
                if (!result.Verify())
                {
                    Logger.Log(LogLevel.Error, LogCategory.Licensing, "License file could not be verified.");
                    result = null;
                }
            }
            catch (Exception Ex)
            {
                Logger.Log(LogLevel.Error, LogCategory.Licensing, "License file failed to load with error: {0}", Ex.Message);
            }
            return result;
        }
    }
}