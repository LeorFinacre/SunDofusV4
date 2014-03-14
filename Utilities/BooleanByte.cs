using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SunDofus.Utilities
{
    class BooleanByte
    {
        public static int SetFlag(int number, int flag, bool value)
        {
            switch (flag)
            {
                case 0:
                    if (value)
                        return number | 1;
                    else
                        return number & (255 - 1);

                case 1:
                    if (value)
                        return number | 2;
                    else
                        return number & (255 - 2);

                case 2:
                    if (value)
                        return number | 4;
                    else
                        return number & (255 - 4);

                case 3:
                    if (value)
                        return number | 8;
                    else
                        return number & (255 - 8);
                case 4:
                    if (value)
                        return number | 16;
                    else
                        return number & (255 - 16);

                case 5:
                    if (value)
                        return number | 32;
                    else
                        return number & (255 - 32);

                case 6:
                    if (value)
                        return number | 64;
                    else
                        return number & (255 - 64);

                case 7:
                    if (value)
                        return number | 128;
                    else
                        return number & (255 - 128);
            }

            return -1;
        }

        public static bool GetFlag(int number, int flag)
        {
            switch (flag)
            {
                case 0:
                    return (number & 1) != 0;

                case 1:
                    return (number & 2) != 0;

                case 2:
                    return (number & 4) != 0;

                case 3:
                    return (number & 8) != 0;

                case 4:
                    return (number & 16) != 0;

                case 5:
                    return (number & 32) != 0;

                case 6:
                    return (number & 64) != 0;

                case 7:
                    return (number & 128) != 0;
            }

            return false;
        }
    }
}