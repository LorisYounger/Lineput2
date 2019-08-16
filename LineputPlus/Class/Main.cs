using System;
using System.Text;
using System.Windows.Media;

//Todo:
//Timer:开始放映的时候回自动开始,显示在右侧
//兼容一部分MarkDown
namespace LineputPlus
{
    public static class Main
    {
        public static readonly string verison = "2.0";
        public static string[] args;
        public static string ColorToHTML(Color Color)
        {
            string returnValue = Convert.ToInt32(Color.R * Math.Pow(256, 2) + Color.G * 256 + Color.B).ToString("x");
            return returnValue.PadLeft(6, '0');
        }
        public static Color HTMLToColor(string HTML = "")
        {
            HTML = HTML.Trim('#').ToLower();
            try
            {
                switch (HTML.Length)
                {
                    case 3:
                        return Color.FromRgb(Convert.ToByte(HTML[0].ToString() + HTML[0], 16), Convert.ToByte(HTML[1].ToString() + HTML[1], 16), Convert.ToByte(HTML[2].ToString() + HTML[2], 16));
                    case 6:
                        return Color.FromRgb(Convert.ToByte(HTML[0].ToString() + HTML[1], 16), Convert.ToByte(HTML[2].ToString() + HTML[3], 16), Convert.ToByte(HTML[4].ToString() + HTML[5], 16));
                    case 8:
                        return Color.FromArgb(Convert.ToByte(HTML[0].ToString() + HTML[1], 16), Convert.ToByte(HTML[2].ToString() + HTML[3], 16), Convert.ToByte(HTML[4].ToString() + HTML[5], 16), Convert.ToByte(HTML[6].ToString() + HTML[7], 16));
                }
            }
            catch
            {

            }
            int hash = Math.Abs(HTML.GetHashCode());
            int hash1 = hash / 256;
            int hash2 = hash1 / 256;
            return Color.FromRgb((byte)(hash % 256), (byte)(hash1 % 256), (byte)(hash2 % 256));
        }
        public static System.Drawing.Color ColorConvent(Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        public static Color ColorConvent(System.Drawing.Color color)
        {
            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        public static int[] CustomColors
        {
            set
            {
                Properties.Settings.Default.CustomColors = string.Join("|", value);
                Properties.Settings.Default.Save();
            }
            get
            {
                string[] vs = Properties.Settings.Default.CustomColors.Split('|');
                int[] vi = new int[vs.Length];
                for (int i = 0; i < vs.Length; i++)
                {
                    vi[i] = Convert.ToInt32(vs[i]);
                }
                return vi;
            }
        }

        /// <summary>
        /// 自动判断文档(bytes)使用的编码
        /// </summary>
        /// <param name="buff">文档源数据</param>
        /// <returns></returns>
        public static Encoding DetectEncoding(byte[] buff)
        {
            if (buff == null || buff.Length < 2)
                return Encoding.Default;      // Default fallback

            if (IsUTF8Bytes(buff) || (buff[0] == 0xEF && buff[1] == 0xBB && buff[2] == 0xBF))
            {
                return Encoding.UTF8;
            }
            else if (buff[0] == 0xFE && buff[1] == 0xFF && buff[2] == 0x00)
            {
                return Encoding.BigEndianUnicode;
            }
            else if (buff[0] == 0xFF && buff[1] == 0xFE && buff[2] == 0x41)
            {
                return Encoding.Unicode;
            }
            return Encoding.Default;

        }
        /// <summary> 
        /// 判断是否是不带 BOM 的 UTF8 格式 
        /// </summary> 
        /// <param name="buff">文档源数据</param> 
        /// <returns></returns> 
        private static bool IsUTF8Bytes(byte[] buff)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数 
            byte curByte; //当前分析的字节. 
            for (int i = 0; i < buff.Length; i++)
            {
                curByte = buff[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前 
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1 
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                throw new Exception("非预期的byte格式");
            }
            return true;
        }


        /// <summary>
        /// 将文档(bytes)转换成文档(string)
        /// </summary>
        /// <param name="fileContent">文档源数据</param>
        /// <returns></returns>
        public static string BytesToString(byte[] fileContent)
        {
            Encoding edi = DetectEncoding(fileContent);
            return edi.GetString(fileContent);
        }
    }

   

   

   
}
