using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace SunDofus.Utilities
{
    class Config
    {
        private static Dictionary<string, string> m_ConfigKey;
        private static string m_Version;

        public static string Version(string nversion = "")
        {
            if (nversion == "")
                return m_Version;

            m_Version = nversion;
            return m_Version;
        }

        public static void LoadConfiguration()
        {
            m_ConfigKey = new Dictionary<string, string>();
            InitializeValues();

            try
            {
                if (!File.Exists("settings.xml"))
                    throw new Exception("Unable to find @'settings.xml'@! The program will use the @basics parameters@!");

                var document = new XmlDocument();
                document.Load("settings.xml");

                foreach (XmlNode node in document.DocumentElement.SelectNodes("/Settings")[0].ChildNodes)
                {
                    var key = node.Name;

                    if (m_ConfigKey.ContainsKey(key))
                        m_ConfigKey.Remove(key);

                    m_ConfigKey.Add(key, node.InnerText);
                }
            }
            catch (Exception exception)
            {
                Logger.Write(Logger.LoggerType.Errors, exception.Message);
            }

            Logger.Write(Logger.LoggerType.Infos, "@'{0}'@ config elements loaded!", m_ConfigKey.Count);
        }

        private static void InitializeValues()
        {
            m_ConfigKey.Add("DEBUG", "TRUE");
            m_ConfigKey.Add("LOGIN_VERSION", "1.29.1");
            m_ConfigKey.Add("LOGIN_IP", "127.0.0.1");
            m_ConfigKey.Add("LOGIN_PORT", "485");
            m_ConfigKey.Add("GAME_ID", "6");
            m_ConfigKey.Add("GAME_IP", "127.0.0.1");
            m_ConfigKey.Add("GAME_PORT", "5555");
        }

        public static int GetInt32(string key)
        {
            return m_ConfigKey.ContainsKey(key) ? int.Parse(m_ConfigKey[key]) : -1;
        }

        public static string GetString(string key)
        {
            return m_ConfigKey.ContainsKey(key) ? m_ConfigKey[key] : "";
        }

        public static bool GetBool(string key)
        {
            return m_ConfigKey.ContainsKey(key) ? bool.Parse(m_ConfigKey[key]) : false;
        }

        public static long GetInt64(string key)
        {
            return m_ConfigKey.ContainsKey(key) ? long.Parse(m_ConfigKey[key]) : -1;
        }

        public static double GetDouble(string key)
        {
            return m_ConfigKey.ContainsKey(key) ? double.Parse(m_ConfigKey[key]) : -1;
        }
    }
}
