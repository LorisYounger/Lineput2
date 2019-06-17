using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using static LineputPlus.Main;
using System.Drawing;
using FontFamily = System.Windows.Media.FontFamily;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Button = System.Windows.Controls.Button;
using RichTextBox = System.Windows.Controls.RichTextBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace LineputPlus
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool Save(bool SaveAs = false)
        {
            if (SaveAs || FilePath == "")
            {
                SaveFileDialog savefile = new SaveFileDialog();
                savefile.Filter = "LPT 文件|*.lpt";
                //Todo:不同功能的后缀
                if (savefile.ShowDialog() == true)
                {
                    FilePath = savefile.FileName;
                    Save(savefile.FileName);                    
                }
                else
                    return false;
            }
            else
                Save(FilePath);
            IsChange = false;
            return true;
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Filter = "LPT 文件|*.lpt";
            //Todo:不同功能的后缀
            // lpt:Lineput文档
            // lps:单页文档,可以插入进lpt(从切换
            // txt:插入文本 从(哪里空着放哪里/最好搞成'插入'在编辑
            // jpg,jpge,png,gif,..:插入图片
            // lptd:打开后直接反映，不进行编辑
            // html:网页(如果有能力就做带切换的(试试js
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
            //刷新当前页面
            RefreshLeftPanelSingle();

            if (NowPage == chosepage)
                return;
            //储存
            LPTED.SavePage(NowPage);

            //跳转
            NowPage = chosepage;
            MarkLeftPanelColor();
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
            cd.CustomColors = CustomColors;
            cd.Color = ColorConvent(LPTED.OADisplay.BackColor);
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LPTED.SavePage(NowPage);//先保存
                LPTED.OADisplay.BackColor = ColorConvent(cd.Color);
                ButtonOABackGroundColor.Background = new SolidColorBrush(LPTED.OADisplay.BackColor);
                LPTED.DisplaySource(NowPage);//重新加载
                                             //重新绘制全部图片
                RefreshLeftPanelAll();
            }
            //储存自定义颜色
            CustomColors = cd.CustomColors;
        }
        private void ButtonOAFontColor_Click(object sender, RoutedEventArgs e)
        {

            ButtonOAFontColor.IsChecked = false;
            ColorDialog cd = new ColorDialog();
            cd.CustomColors = CustomColors;
            cd.Color = ColorConvent(LPTED.OADisplay.FontColor);
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LPTED.SavePage(NowPage);//先保存
                LPTED.OADisplay.FontColor = ColorConvent(cd.Color);
                ButtonOAFontColor.Background = new SolidColorBrush(LPTED.OADisplay.FontColor);
                LPTED.DisplaySource(NowPage);//重新加载
                //重新绘制全部图片
                RefreshLeftPanelAll();
            }
            //储存自定义颜色
            CustomColors = cd.CustomColors;
        }
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
                MessageBox.Show("请输入数字", "设置字体大小");
                return;
            }
            if (LPTED.OADisplay.FontSize.ToString("f1") != fsize.ToString("f1"))
            {
                LPTED.SavePage(NowPage);//先保存

                //储存设置
                LPTED.OADisplay.FontSize = fsize;

                LPTED.DisplaySource(NowPage);//重新加载
                                             //重新绘制全部图片
                RefreshLeftPanelAll();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }
        
        private void ButtonSaveAS_Click(object sender, RoutedEventArgs e)
        {
            Save(true);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //关闭前进行判断
            if (IsChange)
            {
                switch (MessageBox.Show($"是否保存对文件 \"{FileName}\" 的修改", "正在编辑的文件", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        if (!Save())
                            e.Cancel = true;
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                 }
            }
        }

        private void ButtonCreateFile_Click(object sender, RoutedEventArgs e)
        {
            if (IsChange)
            {
                switch (MessageBox.Show($"是否保存对文件 \"{FileName}\" 的修改", "正在编辑的文件", MessageBoxButton.YesNoCancel))
                {
                    case MessageBoxResult.Yes:
                        if (!Save())
                            return;
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            OpenNew();
        }

        private void TextBox1_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            IsChange = true;
        }

        private void ButtonNewPage_Click(object sender, RoutedEventArgs e)
        {
            IsChange = true;
            //先保存当前正在编辑的文档
            LPTED.SavePage(NowPage);
            NowPage++;

            LPTED.EachPage.Insert(NowPage,new LinePutScript.LpsDocument());
            RefreshLeftPanelAll();
            MarkLeftPanelColor();
            TextBox1.Document.Blocks.Clear();
        }

        private void ButtonInsertPage_Click(object sender, RoutedEventArgs e)
        {
            IsChange = true;
            //先保存当前正在编辑的文档
            LPTED.SavePage(NowPage);

            LPTED.EachPage.Insert(NowPage, new LinePutScript.LpsDocument());
            RefreshLeftPanelAll();
            MarkLeftPanelColor();
            TextBox1.Document.Blocks.Clear();
        }

        private void ButtonDeletePage_Click(object sender, RoutedEventArgs e)
        {
            if (LPTED.EachPage.Count == 1)
            {
                MessageBox.Show("最后一页面无法删除","删除失败");
                return;
            }
            if (MessageBox.Show("确认删除该页面吗,此操作无法撤回","删除本页面",MessageBoxButton.YesNo)== MessageBoxResult.Yes)
            {
                
                IsChange = true;
                LPTED.EachPage.RemoveAt(NowPage);
                if (NowPage >= LPTED.EachPage.Count)
                    NowPage = LPTED.EachPage.Count - 1;
                RefreshLeftPanelAll();
                MarkLeftPanelColor();
                LPTED.DisplaySource(NowPage);
            }
        }

        //Todo:IA的应用于更改(在切换栏
    }
}
