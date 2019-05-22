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
                //Todo:重新绘制全部图片
            }
        }
        //Todo:修改全局需要将文档重新载入//可以只载入一个页面 其他的暂时不管了 (除了背景颜色要手动修改
        //Todo:打开文件的时候fontcolor需要应用到textbox1.foreground
        private void ButtonOAFontFamily_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.Font = new Font(LPTED.OADisplay.FontFamily.ToString(), LPTED.OADisplay.FontSize);
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LPTED.SavePage(NowPage);//先保存

                LPTED.OADisplay.FontFamily = new FontFamily(fd.Font.FontFamily.ToString());//储存设置
                LPTED.OADisplay.FontSize = fd.Font.Size;

                ButtonOAFontFamily.Content = "字体:" + fd.Font.FontFamily.ToString();
                LPTED.DisplaySource(NowPage);//重新加载
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
            if (LPTED.OADisplay.FontSize.ToString("f1") != fsize.ToString("f1"))
            {
                LPTED.SavePage(NowPage);//先保存

                //储存设置
                LPTED.OADisplay.FontSize = fsize;

                LPTED.DisplaySource(NowPage);//重新加载
            }
        }


        //Todo:IA的应用于更改(在切换栏
    }
}
