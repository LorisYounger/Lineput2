using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml;
using System.Xml.Xsl;
using System.Linq;
using LinePutScript;
using static LineputPlus.Main;
using System.Text.RegularExpressions;

//Todo:
//Timer:开始放映的时候回自动开始,显示在右侧
//兼容一部分MarkDown
namespace LineputPlus
{
    public static class Main
    {
        public static readonly string verison = "alpha 2";
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
    }

    /// <summary>
    /// 记录一行所用的参数 (实意版本,不是纯文本)
    /// </summary>
    public class LineDisplay
    {
        public float FontSize = 12;
        public Color FontColor = Colors.White;
        public Color BackColor = Color.FromRgb(68, 68, 68);
        public FontFamily FontFamily = new FontFamily("Microsoft YaHei UI");
        public string OutPut = "";//部分可能会用到
        public TextAlignment Alignment = TextAlignment.Left;//有其他属性以paragraph结尾

        public bool Bold = false;
        public bool Italic = false;
        public bool Underline = false;
        public bool Strikethrough = false;

        public LineDisplay()
        {

        }
        public LineDisplay(LineDisplay ld)
        {
            Set(ld);
        }
        /// <summary>
        /// 从别处复制一份参数到本LD,(注意不包含Output)
        /// </summary>
        /// <param name="ld">样本</param>
        public void Set(LineDisplay ld)
        {
            FontSize = ld.FontSize;
            FontColor = ld.FontColor;
            BackColor = ld.BackColor;
            FontFamily = ld.FontFamily;
            Alignment = ld.Alignment;
            Bold = ld.Bold;
            Italic = ld.Italic;
            Underline = ld.Underline;
            Strikethrough = ld.Strikethrough;
        }
        /// <summary>
        /// 判断两个实意行是否有相同的格式 (不对比文本)
        /// </summary>
        /// <param name="ld">被对比的实意行</param>
        /// <returns></returns>
        public bool Equals(LineDisplay ld)
        {
            return !(FontSize != ld.FontSize || FontColor != ld.FontColor || BackColor != ld.BackColor || FontFamily != ld.FontFamily ||
             Alignment != ld.Alignment || Bold != ld.Bold || Italic != ld.Italic || Underline != ld.Underline || Strikethrough != ld.Strikethrough);
        }
        ///在什么情况下用Paragraph或者Run: 有非默认的TextAlignment
        public bool UseRun()
        {
            return Alignment == TextAlignment.Left;
        }
        public bool UseRun(LineDisplay OA)
        {
            return OA.Alignment == TextAlignment.Left;
        }
        public Run OutPutRun()
        {
            Run run = new Run(OutPut);
            if (Bold)
                run.FontWeight = FontWeights.Bold;

            if (Italic)
                run.FontStyle = FontStyles.Italic;

            if (Underline)
                run.TextDecorations = TextDecorations.Underline;

            if (Strikethrough)
                run.TextDecorations = TextDecorations.Strikethrough;

            run.FontSize = FontSize;
            run.FontFamily = FontFamily;
           
            run.Background = new SolidColorBrush(BackColor);
            run.Foreground = new SolidColorBrush(FontColor);
            //run.TextAlignment = Alignment;
            return run;
        }

        public Run OutPutRun(LineDisplay OA)
        {
            Run run = new Run(OutPut);

            if (OA.Bold != Bold)
                if (Bold)
                    run.FontWeight = FontWeights.Bold;

            if (OA.Italic != Italic)
                if (Italic)
                    run.FontStyle = FontStyles.Italic;

            if (OA.Underline != Underline)
                if (Underline)
                    run.TextDecorations = TextDecorations.Underline;

            if (OA.Strikethrough != Strikethrough)
                if (Strikethrough)
                    run.TextDecorations = TextDecorations.Strikethrough;

            if (OA.FontSize != FontSize)
                run.FontSize = FontSize;
            if (OA.FontFamily != FontFamily)
                run.FontFamily = FontFamily;
            if (OA.BackColor != BackColor)
                run.Background = new SolidColorBrush(BackColor);
            if (OA.FontColor != FontColor)
                run.Foreground = new SolidColorBrush(FontColor);
            //run.TextAlignment = Alignment;
            return run;
        }
        //public Paragraph OutPutParagraph(LineDisplay OA)//没有这需求 新建paragraph的时候是没有格式的，使用OA会呈现错误的格式
        //{
        //    Paragraph run = new Paragraph(new Run(OutPut));

        //    if (OA.Bold != Bold)
        //        if (Bold)
        //            run.FontWeight = FontWeights.Bold;

        //    if (OA.Italic != Italic)
        //        if (Italic)
        //            run.FontStyle = FontStyles.Italic;

        //    if (OA.Underline != Underline)
        //        if (Underline)
        //            run.TextDecorations = TextDecorations.Underline;

        //    if (OA.Strikethrough != Strikethrough)
        //        if (Strikethrough)
        //            run.TextDecorations = TextDecorations.Strikethrough;

        //    if (OA.FontSize != FontSize)
        //        run.FontSize = FontSize;
        //    if (OA.FontFamily != FontFamily)
        //        run.FontFamily = FontFamily;
        //    if (OA.BackColor != BackColor)
        //        run.Background = new SolidColorBrush(BackColor);
        //    if (OA.FontColor != FontColor)
        //        run.Foreground = new SolidColorBrush(FontColor);

        //    run.TextAlignment = Alignment;

        //    return run;
        //}

        public Paragraph OutPutParagraph()
        {
            Paragraph run = new Paragraph(new Run(OutPut));
            //run.Inlines.Add();//一个重要的小提示
            if (Bold)
                run.FontWeight = FontWeights.Bold;

            if (Italic)
                run.FontStyle = FontStyles.Italic;

            if (Underline)
                run.TextDecorations = TextDecorations.Underline;

            if (Strikethrough)
                run.TextDecorations = TextDecorations.Strikethrough;

            run.FontSize = FontSize;
            run.FontFamily = FontFamily;
            run.Background = new SolidColorBrush(BackColor);
            run.Foreground = new SolidColorBrush(FontColor);
            run.TextAlignment = Alignment;

            return run;
        }

        /// <summary>
        /// 将行显示行(实意)转换成行(文本) //注 |:| 会直接输出成行
        /// </summary>
        /// <returns>行(文本)</returns>
        public Line ToLine()
        {
            if (OutPut.StartsWith("|") && OutPut.EndsWith(":|"))
                return new Line(OutPut.Substring(1));
            return new Line("linedisplay", "", OutPut, ToSubs());
        }
        /// <summary>
        /// 将显示行(实意)转换成Sub集合
        /// </summary>
        /// <returns>行(文本)</returns>
        public Sub[] ToSubs()
        {
            List<Sub> subs = new List<Sub>();
            if (Bold)
                subs.Add(new Sub("Bold", ""));

            if (Italic)
                subs.Add(new Sub("Italic", ""));

            if (Underline)
                subs.Add(new Sub("Underline", ""));

            if (Strikethrough)
                subs.Add(new Sub("Deleteline", ""));

            subs.Add(new Sub("FontSize", FontSize.ToString("f2")));
            subs.Add(new Sub("FontFamily", FontFamily.ToString()));
            subs.Add(new Sub("BackColor", ColorToHTML(BackColor)));
            subs.Add(new Sub("FontColor", ColorToHTML(FontColor)));
            switch (Alignment)
            {
                case TextAlignment.Left:
                    subs.Add(new Sub("Left", "")); break;
                case TextAlignment.Right:
                    subs.Add(new Sub("Right", "")); break;
                case TextAlignment.Center:
                    subs.Add(new Sub("Center", "")); break;
                case TextAlignment.Justify:
                    subs.Add(new Sub("Justify", "")); break;
            }
            return subs.ToArray();
        }
        /// <summary>
        /// 对多个实意行进行简化处理 (合并同类项)
        /// </summary>
        /// <param name="displays">要被整理的多个实意</param>
        /// <returns></returns>
        public static LineDisplay[] SimplifyLineDisplays(LineDisplay[] displays)
        {
            if (displays.Length <= 1)
                return displays;
            List<LineDisplay> lines = new List<LineDisplay>();
            lines.Add(displays[0]);

            for (int i = 1; i < displays.Length; i++)
            {
                if (displays[i].OutPut.Contains(":|"))
                {
                    if (lines.Last().Equals(displays[i]))
                    {//如果相同就把文本加进去
                        displays[i].OutPut = lines.Last().OutPut + displays[i].OutPut;
                        lines.RemoveAt(lines.Count - 1);
                    }
                    string[] spl = Regex.Split(displays[i].OutPut, @"\:\|", RegexOptions.IgnoreCase);
                    int sp2;
                    for (int i1 = 0; i1 < spl.Length - 1; i1++)
                    {
                        string sp = (string)spl[i1];
                        sp2 = sp.LastIndexOf('|');
                        if (sp2 == -1)
                            lines.Add(new LineDisplay(displays[i]) { OutPut = sp });
                        else if (sp2 == 0)
                        {
                            lines.Add(new LineDisplay() { OutPut = sp + ":|" });
                        }
                        else
                        {
                            lines.Add(new LineDisplay(displays[i]) { OutPut = sp.Substring(0, sp2) });
                            lines.Add(new LineDisplay() { OutPut = sp.Substring(sp2) + ":|" });
                            Console.Write("abc");
                        }
                    }
                }
                else
                {
                    if (lines.Last().Equals(displays[i]))
                    {//如果相同就把文本加进去
                        lines.Last().OutPut += displays[i].OutPut;
                    }
                    else
                    {//不相同就加新的进去
                        lines.Add(displays[i]);
                    }
                }
            }
            return lines.ToArray();
        }
        /// <summary>
        /// 将多个实意转换成Lps文档
        /// </summary>
        /// <param name="displays"></param>
        /// <returns></returns>
        public static LpsDocument LineDisplaysToLpsDocument(LineDisplay[] displays)
        {
            LpsDocument lps = new LpsDocument();
            foreach (LineDisplay ld in displays)
            {
                lps.AddLine(ld.ToLine());
            }
            return lps;
        }



        //xml操作

        /// <summary>
        /// 将FlowDocument(Xaml)转换成实意显示行
        /// </summary>
        /// <param name="xaml">FlowDocument(Xaml)</param>
        /// <param name="OA">全局变量</param>
        /// <returns></returns>
        public static LineDisplay[] XamlToLPT(FlowDocument xaml, LineDisplay OA)
        {
            List<LineDisplay> lps = new List<LineDisplay>();
            MemoryStream ms = new MemoryStream();

            // write XAML out to a MemoryStream
            TextRange tr = new TextRange(
                xaml.ContentStart,
                xaml.ContentEnd);
            tr.Save(ms, DataFormats.Xaml);
            ms.Seek(0, SeekOrigin.Begin);

            XmlDocument xml = new XmlDocument();
            xml.Load(XmlReader.Create(ms, new XmlReaderSettings() { IgnoreComments = true }));

            //读取XAML转换成LPT

            //得到顶层节点列表 
            XmlNodeList topM = xml.DocumentElement.ChildNodes;
            foreach (XmlElement element in topM)
            {
                if (element.Name == "Paragraph")
                {
                    GetLineDisplaysFromLoopXmlElement(element, lps, OA);
                }
            }
            return lps.ToArray();
        }
        /// <summary>
        /// 获取这一xml元素的全部数据
        /// </summary>
        /// <param name="element">ml元素</param>
        /// <param name="OA">全局/临时全局变量</param>
        /// <returns></returns>
        public static LineDisplay GetLineDisplayFormXmlElement(XmlElement element, LineDisplay OA)
        {
            LineDisplay tmp = new LineDisplay(OA);
            foreach (XmlAttribute atr in element.Attributes)
            {
                switch (atr.NodeType)
                {
                    case XmlNodeType.Text:
                        OA.OutPut = atr.Value;
                        break;
                    case XmlNodeType.Attribute:
                        switch (atr.Name)
                        {
                            case "":
                            default:
                                break;

                            case "FontFamily":
                                tmp.FontFamily = new FontFamily(atr.Value);
                                break;
                            case "Foreground":
                                tmp.FontColor = HTMLToColor(atr.Value);
                                break;
                            case "FontSize":
                                tmp.FontSize = Convert.ToSingle(atr.Value);
                                break;
                            case "Background":
                                tmp.BackColor = HTMLToColor(atr.Value);
                                break;
                            case "FontWeight":
                                tmp.Bold = (atr.Value == "Bold"); break;
                            case "FontStyle":
                                tmp.Italic = (atr.Value == "Italic"); break;
                            case "TextDecorations":
                                if (atr.Value == "Strikethrough")
                                    tmp.Strikethrough = !tmp.Strikethrough;
                                else if (atr.Value == "Underline")
                                    tmp.Underline = !tmp.Underline;
                                else
                                {
                                    tmp.Strikethrough = false; tmp.Underline = false;
                                }
                                break;
                            case "TextAlignment":
                                switch (atr.Value)
                                {
                                    case "Left":
                                        tmp.Alignment = TextAlignment.Left; break;
                                    case "Right":
                                        tmp.Alignment = TextAlignment.Right; break;
                                    case "Center":
                                        tmp.Alignment = TextAlignment.Center; break;
                                    case "Justify":
                                        tmp.Alignment = TextAlignment.Justify; break;
                                    default:
                                        tmp.Alignment = TextAlignment.Justify; break;
                                }
                                break;
                        }
                        break;
                        //case XmlNodeType.Attribute:
                        //    tmp =GetLineDisplayFormXmlElement((XmlElement)atr, tmp);
                        //    break;
                }
            }
            return tmp;
        }

        /// <summary>
        /// 循环获取文本输出从xml元素
        /// </summary>
        /// <param name="element">xml元素</param>
        /// <param name="displays">显示行列表</param>
        /// <param name="OA">全局/临时全局变量</param>
        public static void GetLineDisplaysFromLoopXmlElement(XmlElement element, List<LineDisplay> displays, LineDisplay OA)
        {
            //先读这个element
            LineDisplay tmp = GetLineDisplayFormXmlElement(element, OA);
            if (tmp.OutPut != "")//如果有内容就先输出
            {
                displays.Add(tmp);
            }
            foreach (XmlNode xn in element.ChildNodes)
            {
                //由于不可能有注释 直接转换
                //放入循环中进行读取
                switch (xn.NodeType)
                {
                    case XmlNodeType.Text:
                        if (tmp.OutPut == "")
                        {
                            tmp.OutPut = xn.Value;
                            displays.Add(tmp);
                        }
                        else
                        {
                            tmp.OutPut = xn.Value;
                        }
                        break;
                    case XmlNodeType.Element:
                        GetLineDisplaysFromLoopXmlElement((XmlElement)xn, displays, tmp);
                        break;
                    case XmlNodeType.SignificantWhitespace:
                        displays.Add(new LineDisplay(OA) { OutPut = " " });
                        break;
                    default:
                        break;
                }
            }
        }
    }

    //MainWindows的一些类和
    /// <summary>
    /// LPT文件:继承从LPS
    /// </summary>
    public class LptDocument : LpsDocument
    {
        public LptDocument(string LPT) : base(LPT)
        {
            foreach (Sub sub in this.First())
            {
                switch (sub.Name.ToLower())
                {
                    case "allfontcolor":
                    case "fontcolor":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            OADisplay.FontColor = HTMLToColor(sub.info);
                        }
                        break;

                    case "backgroundcolor":
                    case "background":
                    case "allbackcolor":
                    case "backcolor":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            OADisplay.BackColor = HTMLToColor(sub.info);
                        }
                        break;
                    case "fontsize":
                    case "allfontsize":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            OADisplay.FontSize = Convert.ToSingle(sub.info);
                        }
                        break;


                    case "ver":
                    case "verizon":
                        if (sub.info != null)
                        {
                            Verison = sub.info;
                        }
                        break;

                    case "u":
                        OADisplay.Underline = true; break;
                    case "b":
                        OADisplay.Bold = true; break;
                    case "i":
                        OADisplay.Italic = true; break;
                    case "d":
                        OADisplay.Strikethrough = true; break;
                    case "":


                        break;
                    default:
                        FirstLineOtherInfo += $"{sub.Name}#{sub.info}:|";
                        break;
                }
            }
        }


        /// <summary>
        /// 获取文件版本
        /// </summary>
        public string Verison = "-1";
        /// <summary>
        /// 获取第一行除了版本以外的其他全部信息
        /// </summary>
        public string FirstLineOtherInfo = "";
        /// <summary>
        /// 编译错误记录
        /// </summary>
        public StringBuilder log = new StringBuilder();

        /// <summary>
        /// 实际输出用的文本转换,无可逆
        /// </summary>
        /// <param name="Reptex">要被转换的文本</param>
        public void TextReplace(ref string Reptex)
        {
            Reptex = Reptex.Replace("/nowd", DateTime.Now.ToShortDateString());
            Reptex = Reptex.Replace("/nowt", DateTime.Now.ToShortTimeString());
            Reptex = Reptex.Replace("/now", DateTime.Now.ToString());
            Reptex = Reptex.Replace("/lnow", LineNode.ToString());
            Reptex = Reptex.Replace("/llen", Assemblage.Count.ToString());
        }

        public LineDisplay OADisplay = new LineDisplay();
        /// <summary>
        /// 如果为true,跳过一次pageend
        /// </summary>
        public bool IsSkipPageEnd = false;
        /// <summary>
        /// 显示其中一行的内容//只是单纯的显示(编辑用)
        /// </summary>
        /// <param name="line">哪一行的内容是</param>
        /// <param name="fd">要被显示的文档</param>
        public static void DisplayLine(Line line, FlowDocument fd, LineDisplay OAld)
        {
            //Note:
            //pageend等指令插入使用|pageend:|

            line = new Line(line);//复制一个Line//(不会更改提供的内容)
            line.InsertSub(0, line);//Line将会被加进去,以至于可以直接在遍历中找到Line.Name+info
                                    //输出的这一行将会用什么
            LineDisplay disThis = new LineDisplay(OAld);
            foreach (Sub sub in line)
            {
                switch (sub.Name.ToLower())
                {
                    case "":
                    case "linedisplay"://防止自动生成的代码中linedisplay混淆
                        break;
                    //case "cls":Cls不可能出现 出现就是bug
                    //    fd.Blocks.Clear();
                    //    break;
                    case "pageend":
                    case "end":
                        Output(new LineDisplay(OAld) { OutPut = $"|{sub.Name.ToLower()}:|" }, fd);
                        break;
                    case "shell":
                    case "goto":
                    case "img":
                    case "open":
                    case "mov":
                    case "msg":
                        Output(new LineDisplay(OAld) { OutPut = $"|{sub.Name.ToLower()}#{sub.info}:|" }, fd);
                        break;


                    case "font":
                    case "fontfamily":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            disThis.FontFamily = new FontFamily(sub.Info);
                        }
                        break;
                    case "allfont":
                    case "allfontfamily":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            disThis.FontFamily = new FontFamily(sub.Info);
                            OAld.FontFamily = disThis.FontFamily;
                        }
                        break;
                    case "fontcolor":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            disThis.FontColor = HTMLToColor(sub.info);
                        }
                        break;
                    case "allfontcolor":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            disThis.FontColor = HTMLToColor(sub.info);
                            OAld.FontColor = disThis.FontColor;
                        }
                        break;
                    case "fontsize":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到字体大小记录\n");
                        }
                        else
                        {
                            disThis.FontSize = Convert.ToSingle(sub.info);
                        }
                        break;
                    case "allfontsize":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到字体大小记录\n");
                        }
                        else
                        {
                            disThis.FontSize = Convert.ToSingle(sub.info);
                            OAld.FontSize = disThis.FontSize;
                        }
                        break;
                    case "backcolor":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            disThis.BackColor = HTMLToColor(sub.info);
                        }
                        break;
                    case "allbackcolor":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            disThis.BackColor = HTMLToColor(sub.info);
                            OAld.BackColor = disThis.BackColor;
                        }
                        break;
                    case "backgroundcolor":
                        if (sub.info == null)
                        {
                            //log.Append(sub.Name + ":未找到颜色记录\n");
                        }
                        else
                        {
                            fd.Background = new SolidColorBrush(HTMLToColor(sub.info));
                        }
                        break;


                    case "u":
                    case "underline":
                        disThis.Underline = !disThis.Underline; break;
                    case "b":
                    case "bold":
                        disThis.Bold = !disThis.Bold; break;
                    case "i":
                    case "italic":
                        disThis.Italic = !disThis.Italic; break;
                    case "d":
                    case "deleteline":
                        disThis.Strikethrough = !disThis.Strikethrough; break;
                    case "l":
                    case "left":
                        disThis.Alignment = TextAlignment.Left; break;
                    case "r":
                    case "right":
                        disThis.Alignment = TextAlignment.Right; break;
                    case "c":
                    case "center":
                        disThis.Alignment = TextAlignment.Center; break;
                    case "j":
                    case "justify":
                        disThis.Alignment = TextAlignment.Justify; break;

                    //不兼容em
                    case "h1":
                        disThis.FontSize = 48;
                        break;
                    case "h2":
                        disThis.FontSize = 36;
                        break;
                    case "h3":
                        disThis.FontSize = 24;
                        break;
                    case "h4":
                        disThis.FontSize = 16;
                        break;
                    case "h5":
                        disThis.FontSize = 12;
                        break;
                    case "h6":
                        disThis.FontSize = 9;
                        break;

                    default:
                        //支持行内其他代码,为以后的支持插件做准备
                        Output(new LineDisplay(OAld) { OutPut = '|' + sub.ToString() }, fd);
                        break;
                }
            }

            disThis.OutPut += TextDeReplaceMD(line.text);//注:这个是用魔改/stop还是/stop 之所以这么用是因为这个是编辑用不是展示用

            Output(disThis, fd, OAld);//输出
        }
        /// <summary>
        /// 输出内容到FlowDocument
        /// </summary>
        /// <param name="disThis">要输出的内容</param>
        /// <param name="fd">显示的FlowDocument</param>
        /// <param name="OAld">全局,可不填</param>
        public static void Output(LineDisplay disThis, FlowDocument fd, LineDisplay OAld = null)
        {
            //***一个比较有用的案例: (读取的时候也可以使用这个)
            //TextBox1.Document.Blocks.Add(new Paragraph(new Run("内容") { }))
            if (OAld == null)
            {
                if (fd.Blocks.Count != 0 && fd.Blocks.LastBlock.GetType().ToString() == "System.Windows.Documents.Paragraph")
                {
                    ((Paragraph)fd.Blocks.LastBlock).Inlines.Add(disThis.OutPutRun());
                }
                else
                {
                    fd.Blocks.Add(disThis.OutPutParagraph());
                }
            }
            else if (disThis.UseRun(OAld))
            {
                if (fd.Blocks.Count != 0 && fd.Blocks.LastBlock.GetType().ToString() == "System.Windows.Documents.Paragraph")
                {
                    ((Paragraph)fd.Blocks.LastBlock).Inlines.Add(disThis.OutPutRun(OAld));
                }
                else
                {
                    fd.Blocks.Add(disThis.OutPutParagraph());
                }
            }
            else
            {
                fd.Blocks.Add(disThis.OutPutParagraph());
            }
        }
        /// <summary>
        /// 将文本进行反转义处理(正常显示的文本) //魔改版:不更改/stop
        /// </summary>
        /// <param name="Reptex">要反转义的文本</param>
        /// <returns>反转义后的文本 正常显示的文本</returns>
        public static string TextDeReplaceMD(string Reptex)
        {
            Reptex = Reptex.Replace("/tab", "\t");
            Reptex = Reptex.Replace("/n", "\n");
            Reptex = Reptex.Replace("/r", "\r");
            Reptex = Reptex.Replace("/id", "#");
            Reptex = Reptex.Replace("/!", "/");
            Reptex = Reptex.Replace("/com", ",");
            return Reptex;
        }
        /// <summary>
        /// 显示当前阅读行的//包括替换(演讲用)
        /// </summary>
        /// <param name="line">哪一行的内容是</param>
        /// <param name="fd">要被显示的文档</param>
        public void DisplayLine(FlowDocument fd)
        {
            //ToDo
        }

    }


}
