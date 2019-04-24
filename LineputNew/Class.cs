using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Lineput
{

    public class RtfHelper
    {
        public RichTextBox richText = new RichTextBox();
        public static RichTextBox staticrichText = new RichTextBox();

        public void SetRTF(RichTextBox Outrtf) => richText.Rtf = Outrtf.Rtf;
        public void OutRTF(RichTextBox Outrtf) => Outrtf.Rtf = richText.Rtf;


        public void AppendText(string text, Color fontColor, Font font)
        {
            int lastLenth = richText.TextLength;
            richText.AppendText(text);
            richText.Select(lastLenth, text.Length);
            //richText.SelectionStart = lastLenth;
            //richText.SelectionLength = text.Length;
            richText.SelectionColor = fontColor;
            richText.SelectionFont = font;

            //richText.SelectionAlignment = HorizontalAlignment.Left;

            //richText.SelectionStart = richText.TextLength;
        }
        //Output 只有静态输出，动态禁止输出OUTPUT（因为output让大大的改变了参数）
        public static void OutPut(RichTextBox Outrtf, string text, Color fontcolor, Font font)
        {
            staticrichText.Rtf = Outrtf.Rtf;
            int lastLenth = staticrichText.TextLength;
            staticrichText.AppendText(text);
            staticrichText.Select(lastLenth, text.Length);
            staticrichText.SelectionColor = fontcolor;
            staticrichText.SelectionFont = font;
            Outrtf.Rtf = staticrichText.Rtf;
        }

        public static string ColorToHTML(Color Color)
        {
            string returnValue = Convert.ToInt32(Color.R * Math.Pow(256, 2) + Color.G * 256 + Color.B).ToString("x");
            returnValue.PadLeft(6, '0');
            return returnValue;
        }
        public static Color HTMLToColor(string HTML = "")
        {
            if (HTML.Length == 3)
            {
                return Color.FromArgb(Convert.ToInt32("0xff" + HTML[0] + HTML[0] + HTML[1] + HTML[1] + HTML[2] + HTML[2], 16));
            }
            else if (HTML.Length == 6)
            {
                return Color.FromArgb(Convert.ToInt32("0xff" + HTML, 16));
            }
            else
            {
                int hash = Math.Abs(HTML.GetHashCode());
                int hash1 = hash / 256;
                int hash2 = hash1 / 256;
                return Color.FromArgb(hash % 256, hash1 % 256, hash2 % 256);
            }
        }

        public static FontStyle BoolToFontStyle(bool Bsty, bool Delsty, bool Usty, bool Isty)
        {
            FontStyle sty = System.Drawing.FontStyle.Regular;
            if (Bsty)
            {
                sty += 1;
            }
            if (Delsty)
            {
                sty += 8;
            }
            if (Usty)
            {
                sty += 4;
            }
            if (Isty)
            {
                sty += 2;
            }
            return sty;
        }
        public static void FontStyleToBool(FontStyle FontStyle, out bool Bsty, out bool Delsty, out bool Usty, out bool Isty)
        {
            Bsty = false;
            Delsty = false;
            Usty = false;
            Isty = false;
            int tmp = Convert.ToInt32(FontStyle);
            if (tmp >= 8)
            {
                tmp -= 8;
                Delsty = true;
            }
            if (tmp >= 4)
            {
                tmp -= 4;
                Usty = true;
            }
            if (tmp >= 2)
            {
                tmp -= 2;
                Isty = true;
            }
            if (tmp == 1)
            {
                Bsty = true;
            }
        }



    }
    /////// <summary>
    /////// 单个段落 包含格式
    /////// </summary>
    ////public struct Linerf
    ////{// 文字行
    ////    public double FontSize;
    ////    public Color FontColor;
    ////    public FontStyle FontStyle;//新建元素
    ////    public string FontFamily;
    ////    // LPS所需信息

    ////    public Font font()
    ////    {
    ////        return new Font(FontFamily, (float)FontSize, FontStyle);
    ////    }

    ////    string Text;        //文本

    ////    ////Aglin aglin;//等待新版本更新
    ////    ////Underline underline;
    ////    ////Symptom symptom;
    ////    /////// <summary>对齐</summary>
    ////    ////enum Aglin
    ////    ////{
    ////    ////    None,   // (aglin == Aglin.None || <del>对比时如果是None则忽略对比条件 不无视
    ////    ////    Lift,
    ////    ////    Middle,
    ////    ////    Right
    ////    ////}
    ////    /////// <summary>下划线</summary>
    ////    ////enum Underline
    ////    ////{
    ////    ////    None,       //无
    ////    ////    Standard,   //标准
    ////    ////    Dot,        //点
    ////    ////    Thick,      //粗体
    ////    ////    Wave,       //波浪
    ////    ////    Segment     //线段
    ////    ////}
    ////    /////// <summary>标记</summary>
    ////    ////enum Symptom
    ////    ////{
    ////    ////    None,
    ////    ////    SuperScript,    //上标
    ////    ////    SubScript       //下标
    ////    ////}
    ////    /// <summary>对比标记是否相同</summary>
    ////    bool CompareTo(Linerf Another)
    ////    {
    ////        if (FontColor == Another.FontColor && font() == Another.font() &&
    ////            FontSize == Another.FontSize && FontStyle == Another.FontStyle)
    ////        { return true; }
    ////        else { return false; }
    ////    }

    ////}
    /// <summary>
    /// LPS解释器
    /// </summary>
    public class LPSInterpreter
    {
        public RtfHelper helper = new RtfHelper();// 私人解释器，为了防止解释器被一大堆人用。
        public readonly RichTextBox richTextBox;
        public delegate bool Addones(string Order, string[] infs);
        public delegate void Replace(ref string Outtext);
        public Replace reply;
        public Addones addon;
        // LPS信息 默认值
        public double OAFontSize = 16;
        public Color OAFontColor = Color.White;
        public FontStyle OAFontStyle = FontStyle.Regular;
        public string OAFontFamily = "华文楷体";
        // LPS所需信息
        /// <summary>
        /// 文件内容
        /// </summary>
        public string[] linePutLin;
        public int MaxLen;
        /// <summary>
        /// 当前行
        /// </summary>
        public int linePutLinNow;
        /// <summary>
        /// 文件版本
        /// </summary>
        public double FileVerizon = -1;
        /// <summary>
        /// 文件储存时间
        /// </summary>
        public DateTime FileVerizonDate;
        /// <summary>
        /// 是否自动运行
        /// </summary>
        public bool IsAutoRun = false;
        /// <summary>
        /// 编译错误记录
        /// </summary>
        public StringBuilder log = new StringBuilder();
        public Font OAFont()
        {
            return new Font(OAFontFamily, (float)OAFontSize, OAFontStyle);
        }
        public void TextReplace(ref string Reptex)
        {
            Reptex = Reptex.Replace("/|", ":|");
            Reptex = Reptex.Replace("/tab", "\t");
            Reptex = Reptex.Replace("/nowd", DateTime.Now.ToShortDateString());
            Reptex = Reptex.Replace("/nowt", DateTime.Now.ToShortTimeString());
            Reptex = Reptex.Replace("/now", DateTime.Now.ToString());
            Reptex = Reptex.Replace("/n", "\n");
            Reptex = Reptex.Replace("/lnow", linePutLinNow.ToString());
            if (linePutLin != null)
            {
                Reptex = Reptex.Replace("/llen", linePutLin.Length.ToString());
            }
            Reptex = Reptex.Replace("/id", "#");
            Reptex = Reptex.Replace("/?", "//");
            Reptex = Reptex.Replace("/!", "/");
        }
        public LPSInterpreter(ref RichTextBox textBox)
        {
            richTextBox = textBox;
            helper.richText.Rtf = textBox.Rtf;
        }
        /// <summary>
        /// 直接输出文本到RTFBOX
        /// </summary>
        /// <param name="text">输入文本</param>
        /// <param name="fontcolor">文本颜色</param>
        /// <param name="font">文本字体</param>
        public void OutPut(string text, Color fontcolor = new Color(), Font font = null)
        {
            if (fontcolor.IsEmpty)
            {
                fontcolor = OAFontColor;
            }
            if (font == null)
            {
                font = OAFont();
            }

            int str = richTextBox.SelectionStart;
            int len = richTextBox.SelectionLength;
            RtfHelper.OutPut(richTextBox, text, fontcolor, font);
            richTextBox.Select(str, len);
        }
        /// <summary>
        /// 下一行
        /// </summary>
        public int DeLoop = 0;
        public void GoNext()
        {
            if (linePutLin == null)
            {
                OutPut("文件内容为空\n");
                return;
            }
            if (linePutLin.Length <= linePutLinNow)
            {

                OutPut("文件播放完毕 感谢使用Lineput\n");
                return;
            }
            else if (linePutLinNow == 0)
            {//这里只检查是否继续运行，HsCode或者Verizon在打开文件中Check
                if (IsAutoRun)
                {
                    linePutLinNow++;
                    GoNext();
                }
                else
                {
                    linePutLinNow++;
                    IsAutoRun = true;
                }
                return;
            }
            else if (linePutLin[linePutLinNow] == "")
            {
                if (linePutLin.Length == linePutLinNow + 1)
                {
                    OutRTF();
                }
                linePutLinNow++;
                GoNext();
                return;
            }
            DeLoop++;
            if (DeLoop == 200)
            {
                OutPut("Endless loop warning死循环警告\n当重复运行125次时将会强制结束循环\n输入Loop清空循环计数\n", Color.Orange);
                if (MessageBox.Show("死循环警告\n当重复运行125次时将会强制结束循环\n输入Loop清空循环计数", "", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Exclamation) == DialogResult.Abort)
                {
                    DeLoop = 200;
                    return;
                }
            }
            else if (DeLoop == 400)
            {
                MessageBox.Show($"死循环错误\n强制结束循环并退出LPT文件\n错误语句: {linePutLinNow}#{linePutLin[linePutLinNow]}", "Endless loop warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                OutPut($"Endless loop Error死循环错误\n强制结束循环并退出LPT文件\n错误语句: {linePutLinNow}#{linePutLin[linePutLinNow]}\n\n", Color.Red);
                DeLoop = 0;
                return;
            }
            //+历史记录

            if (Interpreter(linePutLin[linePutLinNow]))
            {
                linePutLinNow++;
                GoNext();
                return;
            }
            else
            {
                OutRTF();
                linePutLinNow++;
                return;
            }

        }
        private bool IsOut = false;
        /// <summary>
        /// 缩放方法
        /// ZoomFactor:自带缩放
        /// FontSize 调整字体大小
        /// </summary>
        public ZoomMods zoomMod = ZoomMods.ZoomFactor;
        public enum ZoomMods
        {
            FontSize,//通过字体大小进行缩放:不需要刷新,不会因为改变屏幕而改变
            ZoomFactor//如果过于频繁可能会闪屏
        }
        public void OutRTF()
        {
            //string tmp = richTextBox.ZoomFactor.ToString();
            helper.OutRTF(richTextBox);//richTextBox.Rtf = helper.richText.Rtf;
            if (zoomMod==ZoomMods.ZoomFactor) { addon("zoomck", new string[0]); }
            IsOut = true;
        }
        /// <summary>
        /// 解释器 行解释器 返回是否跳到下一行
        /// </summary>
        /// <param name="Order">命令行</param>
        public bool Interpreter(string Order)
        {
            string[] ods = new string[2];   //分为2的任务(包括命令和信息)
            string[] infs;      //提供信息
            string Outputtext;
            //Order.Replace()
            reply?.Invoke(ref Order);
            TextReplace(ref Order);

            ods = System.Text.RegularExpressions.Regex.Split(Order, "//", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Order = ods[0];

            string[] keys = System.Text.RegularExpressions.Regex.Split(Order, @"\:\|", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            double MyFontsize = OAFontSize; //
            Color MyFontColor = OAFontColor; //
            string MyFontFamily = OAFontFamily;
            bool GoToNext = true; //是否跳到下一行
            bool Bsty = false; //是否加粗
            bool Isty = false; //是否斜体
            bool Usty = false; //是否下划线
            bool Delsty = false; //是否删除
            for (int i = 0; i < keys.Length - 1; i++)
            {
#if !DEBUG
                try{
#endif
                ods = keys[i].Split('#');
                ods[0] = ods[0].ToLower();
                if (ods.Length == 2)
                {
                    infs = ods[1].Split(',');
                }
                else
                {
                    infs = null;
                }
                switch (ods[0])
                {
                    case "":
                        break;

                    case "fontcolor":
                    case "allfontcolor":
                        if (infs == null)
                        {
                            log.Append(ods[0] + ":未找到颜色记录\n");
                        }
                        else
                        {
                            MyFontColor = RtfHelper.HTMLToColor(ods[1]);
                            if (ods[0] != "fontcolor")
                            {
                                OAFontColor = MyFontColor;
                                helper.richText.BackColor = OAFontColor;
                            }
                        }
                        break;
                    case "fontsize":
                    case "allfontsize":
                    case "em":
                        if (infs == null)
                        {
                            log.Append(ods[0] + ":请输入需要设置字体大小\n");
                        }
                        else
                        {
                            MyFontsize = Convert.ToDouble(ods[1]);
                            if (ods[0] == "em")
                            {
                                MyFontsize *= 7.5;
                                log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                            }
                            else if (ods[0] != "fontsize")
                            {
                                OAFontSize = MyFontsize;
                                helper.richText.Font = OAFont();
                            }
                        }
                        break;
                    case "u":
                        Usty = !Usty; break;
                    case "b":
                        Bsty = !Bsty; break;
                    case "i":
                        Isty = !Isty; break;
                    case "d":
                        Delsty = !Delsty; break;
                    case "font":
                    case "allfont":
                        if (infs == null)
                        {
                            log.Append(ods[0] + ":请输入需要设置字体\n");
                        }
                        else
                        {
                            MyFontFamily = ods[1];
                            if (ods[0] != "font")
                            {
                                OAFontFamily = MyFontFamily;
                                helper.richText.Font = OAFont();
                            }
                        }
                        break;
                    case "h1":
                        MyFontsize = 48;
                        break;
                    case "h2":
                        MyFontsize = 36;
                        break;
                    case "h3":
                        MyFontsize = 24;
                        break;
                    case "h4":
                        MyFontsize = 16;
                        break;
                    case "h5":
                        MyFontsize = 12;
                        break;
                    case "h6":
                        MyFontsize = 9;
                        break;

                    //兼容em缩放
                    case "e1":
                        MyFontsize = 60; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e2":
                        MyFontsize = 37.5; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e3":
                        MyFontsize = 22.5; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e4":
                        MyFontsize = 15; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e5":
                        MyFontsize = 9.7; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e6":
                        MyFontsize = 7.5; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e7":
                        MyFontsize = 5.2; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e8":
                        MyFontsize = 3.7; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;
                    case "e9":
                        MyFontsize = 2.2; log.Append(ods[0] + ":兼容旧版本em格式,但不符合LPS2.0规范,请使用FontSize代替.\n");
                        break;




                    case "pageend":
                        //pagend 同时输出文本
                        //helper.OutRTF(richTextBox);
                        GoToNext = false;
                        break;
                    case "sleep"://实在不行讲究下把保留字提到default
                        GoToNext = false;
                        goto default;
                    case "end":
                        linePutLinNow = linePutLin.Length;
                        return false;


                    //将接口转交给addones
                    default:
                        bool Run = addon?.Invoke(ods[0], infs) ?? false;
                        if (!Run) { log.Append(ods[0] + ":找不到对应的LineKey,可能是版本过旧或缺少插件?"); }
                        break;
                }
#if !DEBUG
                }catch(Exception ex)
                {
                    log.Append(ods[0] +":"+ex.Message+'\n');
                }
#endif

            }
            Outputtext = keys[keys.Length - 1];
            if (Outputtext != "")
            {
                FontStyle fs = RtfHelper.BoolToFontStyle(Bsty, Delsty, Usty, Isty);
                Font nf = new Font(MyFontFamily, (float)MyFontsize, fs);
                if (IsOut)
                {
                    IsOut = false;
                    helper.SetRTF(richTextBox);
                }
                helper.AppendText(Outputtext, MyFontColor, new Font(MyFontFamily, (float)MyFontsize, RtfHelper.BoolToFontStyle(Bsty, Delsty, Usty, Isty)));
            }
            if (linePutLin.Length == linePutLinNow + 1)
            {
                OutRTF();
            }
            return GoToNext;
        }



    }
}

