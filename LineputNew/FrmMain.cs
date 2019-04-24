using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Lineput;

namespace LineputNew
{
    public partial class FrmMain : Form
    {
        /// <summary>
        /// 主窗口的LPS解释器控件
        /// </summary>
        public LPSInterpreter LPSbox1;
        /// <summary>
        /// 插件
        /// </summary>
        public string[] AddOnes;
        /// <summary>
        /// 彩蛋
        /// </summary>
        public string[] Eggs;
        /// <summary>
        /// 子窗体
        /// </summary>
        public List<Form> Children = new List<Form>();



        /// <summary>
        /// 窗体引导
        /// </summary>
        public FrmMain()
        {
            //不能删除 初始化窗体
            InitializeComponent();
            //加载addones插件
            FileInfo addones = new FileInfo(Environment.CurrentDirectory + @"\addones.txt");
            if (addones.Exists)
            {
                StreamReader sr = new StreamReader(addones.OpenRead(), System.Text.Encoding.Default);
                AddOnes = sr.ReadToEnd().Replace("\r", "").Split('\n');
                sr.Dispose();
            }
            //加载彩蛋
            FileInfo egg = new FileInfo(Environment.CurrentDirectory + @"\egg.txt");
            if (egg.Exists)
            {
                StreamReader sr = new StreamReader(egg.OpenRead(), System.Text.Encoding.Default);
                Eggs = sr.ReadToEnd().Replace("\r", "").Split('\n');
                sr.Dispose();
            }
            //激活计时器
            timerNext.Tick += TimerNext_Elapsed;


            //初始化LPS解释器
            LPSbox1 = new LPSInterpreter(ref TextBox1)
            {
                addon = LineputFunction,
                reply = MainReplace
            };

            //输出版本信息
            LPSbox1.OutPut($"{Program.Symbol} Ver.{String.Format("{0:N2}", Program.Verizon)} RE:{Program.PublishTime.ToShortDateString()}\n");
            this.Text = "LinePut " + String.Format("{0:N2}", Program.Verizon);
            TextBox1.SelectionStart = TextBox1.TextLength;


            //所有初始加载事件完成

            //如有文件打开
            switch (Program.args.Length)
            {
                case 1:
                    OpenFile(Program.args[0]);
                    break;
                case 0:
                    break;
                default:
                    LPSbox1.OutPut(Program.args[0]);
                    break;

            }
            Program.args = new string[0];
        }
        /// <summary>
        /// 主窗体替换方法
        /// </summary>
        /// <param name="Rept"></param>
        public void MainReplace(ref string Rept)
        {
            Rept = Rept.Replace("/docpath", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            Rept = Rept.Replace("/deskpath", Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory));
            Rept = Rept.Replace("/ProgX86path", Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            Rept = Rept.Replace("/path", Environment.CurrentDirectory);
            if (AddOnes != null)
            {
#if !DEBUG
                try
                {
#endif
                string[] vs = new string[2];

                foreach (string AddOne in AddOnes)
                {
                    vs = AddOne.Split('#');
                    Rept = Rept.Replace(vs[0], vs[1]);
                }
#if !DEBUG
                }
                catch (Exception ex)
                {
                    LPSbox1.OutPut("插件错误:" + ex.Message + '\n', Color.Red);
                }
#endif
            }
        }
        /// <summary>
        /// 查看文件版本
        /// </summary>
        public void FileVerizon()
        {
            LPSbox1.OutPut($"当前文件版本{LPSbox1.FileVerizon} RE:{LPSbox1.FileVerizonDate.ToShortDateString()}\n");
            if (LPSbox1.FileVerizon == -1)
            {
                LPSbox1.OutPut("没有检测到版本信息，可能是旧版本LPT文件，将会产生兼容性问题\n", Color.Red);
            }
            else if (LPSbox1.FileVerizon > Program.Verizon)
            {
                LPSbox1.OutPut("文件版本高于当前LinePut版本，可能会产生兼容性问题，请前往 http://download.exlb.org/?rootPath=./LineputNew 获取Lineput'New'最新版本\n", Color.Yellow);
            }
            else if (LPSbox1.FileVerizon < 2)
            {
                MessageBox.Show("文件版本过低，将会产生严重的兼容性问题，请前往 http://download.exlb.org/?rootPath=./Lineput 获取旧版本Lineput", "过低的文件版本", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                LPSbox1.OutPut("文件版本过低，将会产生严重的兼容性问题，请前往 http://download.exlb.org/?rootPath=./Lineput 获取旧版本Lineput\n", Color.Red);
            }
            else if (LPSbox1.FileVerizon < Program.Verizon)
            {
                LPSbox1.OutPut("文件版本低于当前LinePut版本，可能会产生兼容性问题\n", Color.Yellow);
            }
        }
        /// <summary>
        /// LPT(指主窗体)的自带功能
        /// </summary>
        /// <param name="Order">命令</param>
        /// <param name="info">命令参数</param>
        public bool LineputFunction(string Order, string[] infs = null)
        {

            switch (Order)
            {
                case "":
                    return false;
                case "msg":
                    FrmMain frmMain = new FrmMain();
                    Children.Add(frmMain);
                    frmMain.Zoom = 2;
                    frmMain.Width = 333;
                    frmMain.Height = 250;

                    if (infs == null)
                    {
                        frmMain.TextBox1.Text = "";
                        frmMain.Show();
                    }
                    else
                    {
                        frmMain.TextBox1.Text = infs[0];
                        frmMain.Show();
                    }
                    break;
                case "backcolor": //在addones中使用//背景颜色参数
                    if (infs == null)
                    {
                        LPSbox1.log.Append(Order + ":未找到颜色记录\n");
                    }
                    else
                    {
                        TextBox1.BackColor = RtfHelper.HTMLToColor(infs[0]);
                    }
                    break;
                case "verizon"://查看软件版本
                    LPSbox1.OutPut($"LinePut {Program.Symbol} Ver.{String.Format("{0:N2}", Program.Verizon)} RE:{Program.PublishTime.ToShortDateString()}\n");
                    break;
                case "fileverizon"://查看文件版本
                    FileVerizon();
                    break;
                case "exit"://退出Lineput
                    this.Close();
                    break;
                case "next"://下一行/下一步
                    LPSbox1.GoNext();
                    break;
                case "back"://返回（历史）

                    break;
                case "open"://打开文件
                    if (infs == null)
                    {
                        OpenFile();
                    }
                    else
                    {
                        OpenFile(infs[0]);
                    }
                    break;
                case "zoom"://缩放开关+设置缩放倍数
                    double zoom = Convert.ToDouble(infs[0]);
                    if ((zoom >= 0.2) && (zoom <= 10))
                    {
                        CanZoomChang = true;
                        Zoom = zoom;
                        ZoomChang();
                    }
                    else
                    {
                        CanZoomChang = false;
                    }
                    break;
                case "zoomed"://固定缩放大小
                    double zoomed = Convert.ToDouble(infs[0]);
                    if ((zoomed >= 0.2) && (zoomed <= 10))
                    {
                        TextBox1.ZoomFactor = (float)zoomed;
                        CanZoomChang = false;
                    }
                    else if ((zoomed <= -0.2) && (zoomed >= -10))
                    {
                        TextBox1.ZoomFactor = -(float)zoomed;
                    }
                    break;
                case "zoomck"://检查缩放
                    ZoomChang();
                    break;
                case "sleep"://等待x秒执行
                    StarttimerNext(Convert.ToInt32(infs[0]));
                    break;
                case "shell"://运行某个文件
                    System.Diagnostics.Process.Start(infs[0]);
                    break;
                case "error"://主动抛出错误(用于测试)
                    throw new Exception("测试错误,无效的错误测试信息", new Exception("测试错误 发生在ERROR"));
                case "rtfout"://输出缓存中的文本
                    LPSbox1.helper.OutRTF(TextBox1);
                    break;
                case "goto"://前往
                    LPSbox1.linePutLinNow = Convert.ToInt32(infs[0]);
                    break;
                case "cls"://刷新（清空文本）
                    LPSbox1.helper.richText.ResetText();
                    TextBox1.ResetText();
                    break;
                case "loop"://取消循环计数 取消死循环
                    LPSbox1.DeLoop = 0;
                    break;
                case "hash"://得到Hash值
                    if (LPSbox1.linePutLin != null)
                    {
                        string tmp = LPSbox1.linePutLin[0];
                        LPSbox1.linePutLin[0] = "";
                        LPSbox1.OutPut(LPSbox1.linePutLin.GetHashCode() + "\n");
                        LPSbox1.linePutLin[0] = tmp;
                    }
                    else
                    {

                    }
                    break;
                case "debuginfo"://输出所有的BUG信息
                    LPSbox1.richTextBox.Text = LPSbox1.log.ToString();

                    break;



                default:
                    if (Eggs != null)
                    {
#if !DEBUG
                        try
                        {
#endif
                        string[] vs = new string[2];

                        foreach (string egg in Eggs)
                        {
                            vs = egg.Split('#');
                            if (Order == egg)
                            {
                                LPSbox1.OutPut(vs[1]);
                                return true;
                            }
                        }
#if !DEBUG
                        }
                        catch (Exception ex)
                        {
                            LPSbox1.OutPut("插件错误:" + ex.Message + '\n', Color.Red);
                        }
#endif
                    }
                    return false;
            }
            return true;
        }
        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = @"支持打开的文件|*lpt;*.lps;*.rtf;*.txt;*.jpg;*.jpge;*.png;*.lpp;*.gif;*.bmp;*.mp3;*.mp4;*.mov;*.wav;*.avi;*.mpge|Lineput文件(*.lpt)|*.lpt|Lineput格式文本(*.lps)|*.lps|微软富文本格式(*.rtf)|*.rtf|文本文档(*.txt)|*.txt|图片文件|*.jpg;*.jpge;*.png;*.lpp;*.gif;*.bmp|视频文件|*.mp3;*.mp4;*.mov;*.wav;*.avi;*.mpge",
                Title = "请选择要打开的文件",
                ShowHelp = true
            };
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                //string opfp = openFileDialog.FileName;
                //openFileDialog.Dispose();
                //openFileDialog = null;
                OpenFile(openFileDialog.FileName);
            }
        }
        public void OpenFile(string Path)
        {
#if !DEBUG
            try
            {
#endif
            FileInfo openfile = new FileInfo(Path);
            if (!openfile.Exists) { openfile = new FileInfo(Environment.CurrentDirectory + '\\' + Path); }
            if (!openfile.Exists) { openfile = new FileInfo(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + '\\' + Path); }
            if (!openfile.Exists) { LPSbox1.OutPut($"找不到文件:{Path}\n", Color.Red); return; }
            StreamReader sr;
            switch (openfile.Extension.ToLower())
            {
                //文本
                case ".lpt":
                case ".lps":
                    sr = new StreamReader(openfile.OpenRead(), System.Text.Encoding.Default);
                    LPSbox1.OutPut($"文件{ Path }已打开\n", Color.Yellow);

                    LPSbox1.linePutLin = sr.ReadToEnd().Replace("\r", "").Split('\n');
                    sr.Dispose();
                    sr = null;
                    LPSbox1.MaxLen = Convert.ToInt32(openfile.Length);
                    string[] fls = System.Text.RegularExpressions.Regex.Split(LPSbox1.linePutLin[0], @"\:\|", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    string[] bs = new string[2];

                    //初始化数据
                    LPSbox1.FileVerizon = -1;
                    LPSbox1.linePutLinNow = 0;
                    LPSbox1.IsAutoRun = false;
                    LPSbox1.FileVerizonDate = openfile.LastWriteTime;

                    //开始判断
                    StartPD:
                    foreach (string fl in fls)
                    {
                        bs = fl.Split('#');
                        switch (bs[0].ToLower())
                        {
                            case "":
                                break;
                            case "autorun":
                                LPSbox1.IsAutoRun = true;
                                break;
                            case "verizon":
                                LPSbox1.FileVerizon = Convert.ToDouble(bs[1]);
                                break;
                            case "fullscreen":
                                固定ToolStripMenuItem.Checked = true;
                                this.FormBorderStyle = FormBorderStyle.None;
                                break;
                            case "password":
                                //未完成 str2加密版
                                goto StartPD;
                            case "hash":
                                string tmp = LPSbox1.linePutLin[0];
                                LPSbox1.linePutLin[0] = "";
                                if (LPSbox1.linePutLin.GetHashCode().ToString() != bs[1])
                                {
                                    LPSbox1.OutPut("HASH校验不通过,打开操作被取消。\n 如需继续，请删除 HASH 标签", Color.Red);
                                    LPSbox1.linePutLin = null;
                                    return;
                                }
                                LPSbox1.linePutLin[0] = tmp;
                                break;

                        }
                    }
                    FileVerizon();
                    LPSbox1.GoNext();
                    break;
                case ".rtf":
                    TextBox1.LoadFile(Path);
                    break;
                case ".txt":
                    sr = new StreamReader(openfile.OpenRead(), System.Text.Encoding.Default);
                    LPSbox1.OutPut(sr.ReadToEnd() + '\n');
                    sr.Dispose();
                    break;

                //数据
                case ".jpg":
                case ".png":
                case ".jpge":
                case ".lpp":
                case ".gif":
                case ".bmp":



                    break;

                case ".mov":
                case ".mp4":
                case ".mp3":
                case ".mpge":
                case ".avi":
                case ".wav":



                    break;
            }


            this.Text = openfile.Name + " - Lineput " + String.Format("{0:N2}", Program.Verizon);
#if !DEBUG
            }
            catch (Exception ex)
            {
                LPSbox1.OutPut("插件错误:" + ex.Message + '\n', Color.Red);
            }
#endif
        }


        private Timer timerNext = new Timer
        {
            Enabled = false,
        };

        private void TimerNext_Elapsed(object source, EventArgs e)
        {
            timerNext.Enabled = false;
            LPSbox1.GoNext();
        }
        private void StarttimerNext(int tick)
        {
            timerNext.Interval = tick;
            timerNext.Start();
        }









        //控件部分

        #region"点击控件"
        private void ButtonShowTool_MouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left)
            {
                //这里一定要判断鼠标左键按下状态，否则会出现一个很奇葩的BUG，不信邪可以试一下~~
                //this.Text = $"{MousePos.X - Control.MousePosition.X}  {MousePos.Y - Control.MousePosition.Y}";
                int bx, by, px, py;
                bx = -MousePos.X + Control.MousePosition.X + ShowToolPos.X;
                by = -MousePos.Y + Control.MousePosition.Y + ShowToolPos.Y;
                px = bx - 100;
                py = by - 105;

                if (bx < 0) { bx = 0; }
                if (bx > this.Size.Width - 39) { bx = this.Size.Width - 39; }
                if (by < 0) { by = 0; }
                if (by > this.Size.Height - 62) { by = this.Size.Height - 62; }

                if (px < 0) { px = 0; }
                if (px > this.Size.Width - 139) { px = this.Size.Width - 139; }
                if (py < 0) { py = 0; }
                if (py > this.Size.Height - 167) { py = this.Size.Height - 167; }
                if (by < 105) { py = by + 24; }

                ButtonShowTool.Left = bx;
                ButtonShowTool.Top = by;
                PanelTool.Left = px;
                PanelTool.Top = py;


                if (!IsMove) { IsMove = true; }
            }
        }
        private Point MousePos;
        private Point ShowToolPos;
        private bool IsMove = false;
        private void ButtonShowTool_MouseDown(object sender, MouseEventArgs e)
        {
            MousePos = Control.MousePosition;
            ShowToolPos = ButtonShowTool.Location;
        }

        private void ButtonShowTool_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Right)
            //{
            //    PanelTool.Visible = !PanelTool.Visible;
            //}
        }

        private void ButtonShowTool_MouseUp(object sender, MouseEventArgs e)
        {
            if (IsMove)
            {
                IsMove = false;
            }
            else
            {
                PanelTool.Visible = !PanelTool.Visible;
            }

        }
        private bool CanZoomChang = true;
        private double Zoom = 1;

        private float SizeToZoom()
        {
            float bs = (float)(Math.Sqrt(Math.Pow(this.Size.Width, 2) + Math.Pow(this.Size.Height, 2)) / 832.8 * Zoom);
            //this.Text = bs.ToString();
            if (bs < 0.2)
            {
                bs = 0.2f;
            }
            else if (bs > 25)
            {
                bs = 25f;
            }
            return bs;
        }
        private void ZoomChang()
        {
            if (CanZoomChang)
            {
                float tmp = SizeToZoom();
                if (TextBox1.ZoomFactor != tmp)
                {
                    TextBox1.ZoomFactor = tmp;
                }
             
            }
        }
        private void FrmMain_SizeChanged(object sender, EventArgs e)
        {

            ZoomChang();
            //设置选择框
            int bx, by, px, py;
            bx = ButtonShowTool.Left;
            by = ButtonShowTool.Top;
            px = bx - 100;
            py = by - 105;

            if (bx < 0) { bx = 0; }
            if (bx > this.Size.Width - 39) { bx = this.Size.Width - 39; }
            if (by < 0) { by = 0; }
            if (by > this.Size.Height - 62) { by = this.Size.Height - 62; }

            if (px < 0) { px = 0; }
            if (px > this.Size.Width - 139) { px = this.Size.Width - 139; }
            if (py < 0) { py = 0; }
            if (py > this.Size.Height - 167) { py = this.Size.Height - 167; }
            if (by < 105) { py = by + 24; }

            ButtonShowTool.Left = bx;
            ButtonShowTool.Top = by;
            PanelTool.Left = px;
            PanelTool.Top = py;
        }
        #endregion

        private void 剪切TToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox1.Cut();
        }

        private void 复制CToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox1.Copy();
        }

        private void 粘贴PToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox1.Paste();
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TextBox1.ResetText();
        }

        private void 字体ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog dialog = new FontDialog
            {
                Font = TextBox1.SelectionFont
            };
            if (dialog.ShowDialog() == DialogResult.OK) { TextBox1.SelectionFont = dialog.Font; }
        }

        private void 字体颜色DToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog
            {
                Color = TextBox1.SelectionColor
            };
            if (dialog.ShowDialog() == DialogResult.OK) { TextBox1.SelectionColor = dialog.Color; }
        }

        private void ButtonCl_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            TextBox1.SelectionColor = button.BackColor;
        }

        private void ButtonFB_Click(object sender, EventArgs e)
        {

        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
#if !DEBUG
            try
            {
#endif
            if ((e.KeyValue == 13) && (TextBox1.SelectionStart == TextBox1.TextLength))
            {
                //string[] boxtext = TextBox1.Text.Split('\n');
                int ind = TextBox1.GetLineFromCharIndex(TextBox1.SelectionStart);
                //LPSbox1.helper.SetRTF(TextBox1);
                //LPSbox1.Interpreter(boxtext[ind]);
                //LPSbox1.helper.OutRTF(TextBox1);

                string[] boxtext = TextBox1.Text.Split('\n');

                // 这里是通过鼠标判断在输入哪里进行判断，但是这样会导致输出顺序复杂
                //int ind = TextBox1.GetLineFromCharIndex(TextBox1.SelectionStart)-1;
                //if (ind >= boxtext.Length) {
                //    ind = boxtext.Length - 1; }

                string[] ods = new string[2];   //分为2的任务(包括命令和信息)
                string[] infs;      //提供信息
                MainReplace(ref boxtext[boxtext.Length - 2]);
                LPSbox1.TextReplace(ref boxtext[boxtext.Length - 2]);
                //Order.Replace()
                foreach (string key in System.Text.RegularExpressions.Regex.Split(boxtext[boxtext.Length - 2], @"\:\|", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
#if !DEBUG
                        try
                        {
#endif
                    ods = key.Split('#');
                    ods[0] = ods[0].ToLower();
                    if (ods.Length == 2)
                    {
                        infs = ods[1].Split(',');
                    }
                    else
                    {
                        infs = null;
                    }
                    LineputFunction(ods[0], infs);
#if !DEBUG
                        }
                        catch (Exception ex)
                        {
                            LPSbox1.OutPut(ods[0] + ":" + ex.Message + '\n', Color.Red);
                        }
#endif
                }
                TextBox1.SelectionStart = TextBox1.TextLength;
            }
#if !DEBUG
            }
            catch (Exception ex)
            {
                LPSbox1.OutPut("键入:" + ex.Message + '\n', Color.Red);
            }
#endif
        }

        private void 全选AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextBox1.SelectAll();
        }

        private void ButtonN_Click(object sender, EventArgs e)
        {
            LPSbox1.GoNext();
        }

        private void 下一行NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LPSbox1.GoNext();
        }

        private void 导入ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }
        public static string ToSizeString(long Size)
        {
            int i = 0;
            while (Size / (Math.Pow(1024, i)) > 1024)
            {
                i++;
            }
            double TrueSize = Convert.ToInt32(Size / (Math.Pow(1024, i)) * 100) * 0.01;
            switch (i)
            {
                case 0:
                    return TrueSize + "B";
                case 1:
                    return TrueSize + "KB";
                case 2:
                    return TrueSize + "MB";
                case 3:
                    return TrueSize + "GB";
                case 4:
                    return TrueSize + "TB";
                case 5:
                    return TrueSize + "PB";
                case 6:
                    return TrueSize + "EB";
                case 7:
                    return TrueSize + "ZB";
                case 8:
                    return TrueSize + "YB";
                default:
                    return TrueSize + "x" + i;
            }
        }

        private void PanelTool_MouseHover(object sender, EventArgs e)
        {
            int zyzy = TextBox1.Rtf.Length + LPSbox1.richTextBox.Rtf.Length + RtfHelper.staticrichText.Rtf.Length + LPSbox1.MaxLen;
            toolTip1.SetToolTip(PanelTool, "占用资源:" + ToSizeString(zyzy * 8));
        }
    }
}
