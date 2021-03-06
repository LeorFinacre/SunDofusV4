﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Utilities
{
    class Basic
    {
        private static int m_StartTime;
        private static Random m_Randomizer = new Random();

        public static List<char> HASH = new List<char>() {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's',
                't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U',
                'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '-', '_'};

        public static int Uptime
        {
            get
            {
                return (Environment.TickCount - m_StartTime);
            }
            set
            {
                m_StartTime = value;
            }
        }

        public static int[] GetUpTime()
        {
            int[] date = new int[3];
            double milli = Uptime;

            date[2] = (int)(milli / 1000);

            if (date[2] > 59)
            {
                date[1] = (int)Math.Round((double)date[2] / 60);
                date[2] = date[2] - (date[1] * 60);

                if (date[0] > 59)
                {
                    date[0] = (int)Math.Round((double)date[1] / 60);
                    date[1] = date[1] - (date[0] * 60);
                }
            }

            return date;
        }

        public static string RandomString(int lenght)
        {
            var str = string.Empty;

            for (var i = 1; i <= lenght; i++)
            {
                var randomInt = m_Randomizer.Next(0, HASH.Count);
                str += HASH[randomInt];
            }

            return str;
        }

        public static string Encrypt(string password, string key)
        {
            var _Crypted = "1";

            for (var i = 0; i < password.Length; i++)
            {
                var PPass = password[i];
                var PKey = key[i];
                var APass = (int)PPass / 16;
                var AKey = (int)PPass % 16;
                var ANB = (APass + (int)PKey) % HASH.Count;
                var ANB2 = (AKey + (int)PKey) % HASH.Count;

                _Crypted += HASH[ANB];
                _Crypted += HASH[ANB2];

            }

            return _Crypted;
        }

        private static string m_Vowels = "aeiouy";
        private static string m_Consonants = "bcdfghjklmnpqrstvwxz";

        public static int Rand(int min, int max)
        {
            return m_Randomizer.Next(min, max + 1);
        }

        public static string GetRandomVowel
        {
            get
            {
                return m_Vowels.Substring((Rand(0, m_Vowels.Length - 1)), 1);
            }
        }

        public static string GetRandomConsonant
        {
            get
            {
                return m_Consonants.Substring((Rand(0, m_Consonants.Length - 1)), 1);
            }
        }

        public static string RandomName()
        {
            var name = GetRandomConsonant.ToUpper();
            name += GetRandomVowel;

            if (Rand(0, 1) == 0)
                name += GetRandomVowel;

            name += GetRandomConsonant;

            if (Rand(0, 1) == 0)
                name += name[name.Length - 1].ToString();

            name += GetRandomVowel;

            if (Rand(0, 1) == 0)
            {
                name += GetRandomConsonant;
                name += GetRandomVowel;
            }

            return name;
        }

        public static string DeciToHex(int deci)
        {
            if (deci == -1)
                return "-1";

            else return deci.ToString("x");
        }

        public static string ToBase36(int num)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int r;
            string newNumber = "";

            while (num >= 36)
            {
                r = num % 36;
                newNumber = chars[r] + newNumber;
                num = num / 36;
            }

            newNumber = chars[num] + newNumber;

            return newNumber;
        }

        public static int HexToDeci(string hex)
        {
            if (hex == "-1" | hex == "")
                return -1;

            return Convert.ToInt32(hex, 16);
        }

        public static string GetActualTime()
        {
            return string.Format("{0}{1}{2}{3}", (DateTime.Now.Hour * 3600000), (DateTime.Now.Minute * 60000),
                (DateTime.Now.Second * 1000), DateTime.Now.Millisecond.ToString());
        }

        public static string GetDofusDate()
        {
            return string.Format("BD{0}|{1}|{2}", (DateTime.Now.Year - 1370).ToString(), (DateTime.Now.Month - 1), (DateTime.Now.Day));
        }

        public static int GetRandomJet(string jetStr, int jet = 4)
        {
            if (jetStr.Length > 3)
            {
                var damages = 0;
                var ds = int.Parse(jetStr.Split('d')[0]);
                var faces = int.Parse(jetStr.Split('d')[1].Split('+')[0]);
                var fixe = int.Parse(jetStr.Split('d')[1].Split('+')[1]);

                if (ds != 0)
                {
                    for (var i = 1; i <= ds; i++)
                    {
                        if (jet == 1)
                            damages += faces;
                        else if (jet == 2)
                            damages += 1;
                        else if (jet == 3)
                            damages += (int)Math.Ceiling((double)(faces / 2));
                        else
                            damages += Rand(1, faces);
                    }
                }
                return (damages + fixe);
            }
            else
                return 0;
        }
    }
}
