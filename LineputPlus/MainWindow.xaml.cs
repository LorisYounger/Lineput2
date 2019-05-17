using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Linq;
using LinePutScript;
using System.Windows.Documents;
using Microsoft.Win32;
using static LineputPlus.Main;
using System.Drawing;
using System.IO;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Button = System.Windows.Controls.Button;
using RichTextBox = System.Windows.Controls.RichTextBox;
using Label = System.Windows.Controls.Label;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;

namespace LineputPlus
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// LPT编辑器文件
        /// </summary>
        public LPTEditor LPTED;
        /// <summary>
        /// 当前页面
        /// </summary>
        public int NowPage;
        public MainWindow()
        {
            InitializeComponent();

            if (args == null)
            {
                OpenLPT("LPT#Origin:|ver#2:|Author#UserName:|\nh3:|欢迎使用Lineput");
            }
            else
            {
                switch (args.Length)
                {
                    case 0:
                        OpenLPT("LPT#Origin:|ver#2:|Author#UserName:|\nh3:|欢迎使用Lineput");
                        break;
                    case 1:
                        OpenFile(args[0]);
                        break;
                    default:
                        MessageBox.Show(args[0]);
                        break;
                }
            }

        }
        public void OpenFile(string path)
        {
            FileInfo openfile = new FileInfo(path);
            if (!openfile.Exists) { openfile = new FileInfo(Environment.CurrentDirectory + '\\' + path); }
            if (!openfile.Exists) { openfile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + '\\' + path); }
            if (!openfile.Exists) { OutPut($"找不到文件:{path}\n", Colors.Red); return; }
            StreamReader sr;
            sr = new StreamReader(openfile.OpenRead(), System.Text.Encoding.Default);
            OutPut($"文件{ path }已打开\n", Colors.Orange);
            string str = sr.ReadToEnd();
            sr.Dispose();
            OpenLPT(str);
            //版本检查
            if (LPTED.Verison.Contains("1.") || LPTED.Verison == "-1")
            {
                OutPut("文件版本过旧,可能不兼容或部分弃用功能无法使用\n", Colors.Red);
            }
        }
        public void OpenLPT(string lpt)
        {
            LPTED = new LPTEditor(TextBox1, lpt);
            //清空全部左窗体
            LeftPanel.Children.Clear();
            Button bt; TextBlock tb;
            for (int i = 0; i < LPTED.EachPage.Count; i++)
            {
                bt = new Button
                {
                    Margin = new Thickness(5, 5, 9, 5),
                    Background = new SolidColorBrush(Color.FromRgb(224, 224, 224))
                };
                tb = new TextBlock();
                tb.Inlines.Add(new System.Windows.Controls.Image()
                {
                    Source = LPTED.DisplayImage(i),
                    Margin = new Thickness(2, 0, 0, 0),
                    Height = 80,
                    Width = 120,
                    Stretch = Stretch.UniformToFill
                });
                tb.Inlines.Add(new LineBreak());
                tb.Inlines.Add(new Label()
                {
                    Margin = new Thickness(7, 0, 0, 0),
                    Content = LPTED.GetTitle(i)
                });
                bt.Content = tb;
                bt.Name = "pg" + i.ToString();
                bt.Click += Bt_Click;
                LeftPanel.Children.Add(bt);
            }
            TextBoxFirstLineOtherInfo.Text = LPTED.FirstLineOtherInfo;
            NowPage = 0;
            LPTED.DisplaySource(NowPage);
        }

        /// <summary>
        /// 获取截图图片 (可以直接当源用)
        /// </summary>
        /// <param name="element">要截图的控件</param>
        /// <returns></returns>
        public static RenderTargetBitmap GenerateImage(FrameworkElement element)
        {
            RenderTargetBitmap bitmapRender = new RenderTargetBitmap((int)element.ActualWidth, (int)element.ActualHeight + 100, 96, 96, PixelFormats.Pbgra32);//位图 宽度  高度   水平DPI  垂直DPI  位图的格式    高度+100保证整个图都能截取
            bitmapRender.Render(element);
            return bitmapRender;
        }

        //private void Window_Loaded(object sender, RoutedEventArgs e)
        //{
        //    DisplayImg.Source = LPTED.DisplayImage(0);
        //    MessageBox.Show(LPTED.DisplayImage(0).Width.ToString());
        //    DisplayImg.Stretch = Stretch.f;
        //    DisplayImg.Source = GenerateImage(TextBox1);
        //    MessageBox.Show(TextBox1.Document.Blocks.LastBlock.GetType().ToString());
        //}
        /// <summary>
        /// 输出文本到控制行
        /// </summary>
        /// <param name="context">文本</param>
        /// <param name="cl">颜色</param>
        /// <param name="fontsize">字体大小</param>
        /// <param name="fontfamily">字体样式</param>
        public void OutPut(string context, Color cl = new Color(), double fontsize = 12, string fontfamily = "Microsoft YaHei UI")
        {
            if (cl.A == 0)
                cl = Colors.White;
            Run run = new Run(context);
            run.FontSize = fontsize;
            run.FontFamily = new FontFamily(fontfamily);
            run.Foreground = new SolidColorBrush(cl);
            if (ConsoleBox.Document.Blocks.Count != 0 && ConsoleBox.Document.Blocks.LastBlock.GetType().ToString() == "System.Windows.Documents.Paragraph")
            {
                ((Paragraph)ConsoleBox.Document.Blocks.LastBlock).Inlines.Add(run);
            }
            else
            {
                ConsoleBox.Document.Blocks.Add(new Paragraph(run));
            }
            if (cl == Colors.Red)
                MessageBox.Show(context, context.Split('\n')[0]);
        }
        public class LPTEditor : LptDocument
        {
            public RichTextBox Document;
            public LPTEditor(RichTextBox fdm, string lpt) : base(lpt)
            {
                Document = fdm;
                //this.Load(lpt);//盲猜不需要读取，lps读过了


                //第一行跳过不读(后续将内容放进来)
                LineNode = 1;

                //将页面进行分割
                Line tmp;
                EachPage.Add(new LpsDocument());
                //所有的cls都是新页面:2.0版规则_cls=pageend+cls
                //如需跳过,使用skip
                while (this.ReadCanNext())
                {
                    tmp = ReadNext();
                    if (tmp.Name.ToLower() == "cls")//cls必须为开头行//回到本质:lpt不追求大小写(但是LPS追求/区分)
                    {
                        EachPage.Add(new LpsDocument());
                    }
                    //作为新的page
                    //EachPage.Last().AddLine(tmp);不加进去，这个是CLS,之后在save的时候再加
                }

            }
            /// <summary>
            /// 每页的内容分别储存好 注:可能和源文件Assemblage不同步,使用Save进行同步
            /// </summary>
            public List<LpsDocument> EachPage = new List<LpsDocument>();

            /// <summary>
            /// 显示某一页的内容(lpt=>flowdocument
            /// </summary>
            /// <param name="Page">页数</param>
            public void DisplaySource(int Page)
            {
                Document.Document.Blocks.Clear();
                Document.Background = new SolidColorBrush(OADisplay.BackColor);
                foreach (Line lin in EachPage[Page])
                {
                    DisplayLine(lin, Document.Document, OADisplay);
                }
            }
            /// <summary>
            /// 显示图片(不精确
            /// </summary>
            /// <param name="Page">页面</param>
            /// <returns></returns>
            public ImageSource DisplayImage(int Page)
            {
                Bitmap bm = new Bitmap(300, 200);
                Graphics g1 = Graphics.FromImage(bm);
                g1.FillRectangle(new SolidBrush(ColorConvent(OADisplay.BackColor)), new Rectangle(0, 0, 300, 200));
                int x = 0;
                int y = 0;
                for (int i = 0; i < EachPage[Page].Assemblage.Count; i++)
                {
                    Line nu = new Line(EachPage[Page].Assemblage[i]);
                    nu.InsertSub(0, nu);//Line将会被加进去,以至于可以直接在遍历中找到Line.Name+info
                    LineDisplay disThis = new LineDisplay(OADisplay);
                    foreach (Sub sub in nu)
                    {
                        switch (sub.Name.ToLower())
                        {
                            case "fontcolor":
                            case "allfontcolor":
                                if (sub.info == null)
                                {
                                    //log.Append(sub.Name + ":未找到颜色记录\n");
                                }
                                else
                                {
                                    disThis.FontColor = HTMLToColor(sub.info);
                                }
                                break;
                            //case "allbackcolor":
                            //case "backcolor":
                            //    if (sub.info == null)
                            //    {
                            //        //log.Append(sub.Name + ":未找到颜色记录\n");
                            //    }
                            //    else
                            //    {
                            //        disThis.BackColor = HTMLToColor(sub.info);
                            //    }
                            //    break;
                            case "fontsize":
                            case "allfontsize":
                                if (sub.info == null)
                                {
                                    //log.Append(sub.Name + ":未找到颜色记录\n");
                                }
                                else
                                {
                                    disThis.FontSize = Convert.ToSingle(sub.info);
                                }
                                break;
                            case "backgroundcolor":
                                if (sub.info == null)
                                {
                                    //log.Append(sub.Name + ":未找到颜色记录\n");
                                }
                                else
                                {
                                    g1.FillRectangle(new SolidBrush(ColorConvent(HTMLToColor(sub.info))), new Rectangle(0, 0, 300, 200));
                                }
                                break;


                            //case "u"://无法在画图中实现 直接干掉
                            //case "underline":
                            //    disThis.Underline = !disThis.Underline; break;
                            //case "b":
                            //case "bold":
                            //    disThis.Bold = !disThis.Bold; break;
                            //case "i":
                            //case "italic":
                            //    disThis.Italic = !disThis.Italic; break;
                            //case "d":
                            //case "deleteline":
                            //    disThis.Strikethrough = !disThis.Strikethrough; break;

                            case "r":
                            case "right":
                                x = 200; break;
                            case "c":
                            case "center":
                                x = 100; break;

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
                        }
                    }

                    g1.DrawString(nu.Text, new Font(disThis.FontFamily.ToString(), disThis.FontSize / 2), new SolidBrush(ColorConvent(disThis.FontColor)), 0, y);
                    if (nu.Text.Contains('\n'))
                    {
                        x = 0;
                        y += Convert.ToInt16(disThis.FontSize / 2 * (nu.Text.Split('\n').Length) - 1);
                    }
                    else
                        x += Convert.ToInt16(disThis.FontSize / 2 * nu.text.Length);

                    if (y >= 250)
                        break;
                }
                //修改源
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bm.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            }
            /// <summary>
            /// 获取标题
            /// </summary>
            /// <param name="Page">页数</param>
            /// <returns></returns>
            public string GetTitle(int Page)
            {
                string sb = "";
                foreach (Line li in EachPage[Page].Assemblage)
                {
                    sb += li.Text;
                    if (sb.Contains('\n'))
                    {
                        if (sb.Split('\n')[0].Trim() == "")
                            sb = sb.Split('\n')[1];
                        else
                        {
                            sb = sb.Split('\n')[0].Trim();
                            if (sb.Length > 15)
                                return sb.Substring(0, 15);
                            else
                                return sb;
                        }
                    }
                }
                sb = sb.Trim();
                if (sb.Trim() == "")
                    return "[未命名]";
                else
                {
                    if (sb.Length > 15)
                        return sb.Substring(0, 15);
                    else
                        return sb;
                }
            }
            /// <summary>
            /// 保存某一页
            /// </summary>
            /// <param name="Page">页数</param>
            public void SavePage(int Page)
            {
                EachPage[Page] = LineDisplay.LineDisplaysToLpsDocument(LineDisplay.SimplifyLineDisplays(LineDisplay.XamlToLPT(Document.Document, OADisplay)));
            }
            /// <summary>
            /// 将编辑内容转换成标准lps,可以直接使用
            /// </summary>
            public void Save()
            {
                Assemblage.Clear();
                Line fl = new Line("LinePut:|ver#" + Verison + ":|" + FirstLineOtherInfo);
                fl.AddRange(OADisplay.ToSubs());
                Assemblage.Add(fl);
                foreach (LptDocument lpt in EachPage)
                {
                    Assemblage.AddRange(lpt.Assemblage);
                    Assemblage.Add(new Line("cls:|"));//加入cls分页符号到每一行
                }
            }
        }



        //--------界面方法----------

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "LPT 文件|*.lpt";
            if (openfile.ShowDialog() == true)
            {
                OpenFile(openfile.FileName);
            }
        }
        /// <summary>
        /// 左侧按钮被选中
        /// </summary>
        /// <param name="sender">按钮</param>
        /// <param name="e">信息</param>
        private void Bt_Click(object sender, RoutedEventArgs e)
        {
            int chosepage = Convert.ToInt32(((Button)sender).Name.Substring(2));
            //MessageBox.Show(chosepage.ToString());
            TextBlock tb = (TextBlock)((Button)LeftPanel.Children[NowPage]).Content;
            //获取tb里面的内容//拿不到,不会
            //((System.Windows.Controls.Image)tb.Inlines.FirstInline.).Source = LPTED.DisplayImage(NowPage);
            //所以干脆就清空了
            tb.Inlines.Clear();
            tb.Inlines.Add(new System.Windows.Controls.Image()
            {
                Source = GenerateImage(TextBox1),
                Height = 80,
                Width = 120,
                Margin = new Thickness(2, 0, 0, 0),
                Stretch = Stretch.UniformToFill
            });
            tb.Inlines.Add(new LineBreak());
            tb.Inlines.Add(new Label()
            {
                Margin = new Thickness(7, 0, 0, 0),
                Content = LPTED.GetTitle(NowPage)
            });

            if (NowPage == chosepage)
                return;
            //储存
            LPTED.SavePage(NowPage);

            //跳转
            NowPage = chosepage;
            LPTED.DisplaySource(NowPage);
        }
        bool AutoCloseToolBar = false;
        private void TabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AutoCloseToolBar = !AutoCloseToolBar;
            if (AutoCloseToolBar)
                ToolBar.Background = new SolidColorBrush(Colors.YellowGreen);
            else
                ToolBar.Background = new SolidColorBrush(Colors.SkyBlue);
            ToolBarHeight.Height = new GridLength(110, GridUnitType.Pixel);
        }

        private void ConsoleBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AutoCloseToolBar = !AutoCloseToolBar;
            if (AutoCloseToolBar)
                ToolBar.Background = new SolidColorBrush(Colors.YellowGreen);
            else
                ToolBar.Background = new SolidColorBrush(Colors.SkyBlue);
            ToolBarHeight.Height = new GridLength(110, GridUnitType.Pixel);
        }

        private void ToolBar_MouseLeave(object sender, MouseEventArgs e)
        {
            if (AutoCloseToolBar)
                ToolBarHeight.Height = new GridLength(30, GridUnitType.Pixel);
        }

        private void ToolBar_MouseEnter(object sender, MouseEventArgs e)
        {
            if (AutoCloseToolBar)
                ToolBarHeight.Height = new GridLength(110, GridUnitType.Pixel);
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Button)sender).Background = new SolidColorBrush(Colors.SkyBlue);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ((RichTextBox)sender).Selection.Select(((RichTextBox)sender).Document.ContentEnd, ((RichTextBox)sender).Document.ContentEnd);
        }

        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            Lineputxaml lineputxaml = new Lineputxaml();
            this.Hide();
            lineputxaml.ShowDialog();
            this.Show();
        }
        //ToDO：完善按钮功能
        private void ButtonOABackGroundColor_Click(object sender, RoutedEventArgs e)
        {
            ButtonOABackGroundColor.IsChecked = false;
            ColorDialog cd = new ColorDialog();
            cd.Color = ColorConvent(LPTED.OADisplay.BackColor);
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LPTED.OADisplay.BackColor = ColorConvent(cd.Color);
                ButtonOABackGroundColor.Background = new SolidColorBrush(LPTED.OADisplay.BackColor);
            }
        }

        private void ButtonOAFontColor_Click(object sender, RoutedEventArgs e)
        {
            ButtonOAFontColor.IsChecked = false;
            ColorDialog cd = new ColorDialog();
            cd.Color = ColorConvent(LPTED.OADisplay.FontColor);
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LPTED.OADisplay.FontColor = ColorConvent(cd.Color);
                ButtonOAFontColor.Background = new SolidColorBrush(LPTED.OADisplay.FontColor);
            }
        }
        //Todo:修改全局需要将文档重新载入//可以只载入一个页面 其他的不管了
        //Todo:每个页面有自己单独的OAdisplay(eg:allfontcolor)
        //Todo:打开文件的时候fontcolor需要应用到textbox1.foreground
        private void ButtonOAFontFamily_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.Font = new Font(LPTED.OADisplay.FontFamily.ToString(), LPTED.OADisplay.FontSize);
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LPTED.OADisplay.FontFamily = new FontFamily(fd.Font.FontFamily.ToString());
                ButtonOAFontColor.Background = new SolidColorBrush(LPTED.OADisplay.FontColor);
            }
        }

        private void ComboBoxOAFontSize_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeFontSize();
        }

        private void ComboBoxOAFontSize_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChangeFontSize();
        }

        private void ComboBoxOAFontSize_DropDownClosed(object sender, EventArgs e)
        {
            ChangeFontSize();
        }
        private void ChangeFontSize()
        {
            if (!float.TryParse(ComboBoxOAFontSize.Text, out float fsize))
            {
                MessageBox.Show("请输入数字","设置字体大小");
                return;
            }
            if (LPTED.OADisplay.FontSize != fsize)
            {
                MessageBox.Show(fsize.ToString() +"\n" + LPTED.OADisplay.FontSize.ToString());
            }
        }
    }
}
