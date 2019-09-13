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
using System.Windows.Documents;

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
        private void AutoCloseToolBarClick()
        {
            AutoCloseToolBar = !AutoCloseToolBar;
            if (AutoCloseToolBar)
                ToolBar.Background = new SolidColorBrush(Colors.YellowGreen);
            else
                ToolBar.Background = new SolidColorBrush(Colors.SkyBlue);
            ToolBarHeight.Height = new GridLength(110, GridUnitType.Pixel);
        }
        private void TabControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AutoCloseToolBarClick();
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
            //先保存当前正在编辑的文档
            LPTED.SavePage(NowPage);
            //将page内容储存到lpt
            LPTED.Save();
            
            Lineputxaml lineputxaml = new Lineputxaml(LPTED.ToString());
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
            cd.Dispose();
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
            cd.Dispose();
        }
        private void ButtonOAFontFamily_Click(object sender, RoutedEventArgs e)
        {
            FontDialog fd = new FontDialog();
            fd.Font = new Font(LPTED.OADisplay.FontFamily.ToString(), LPTED.OADisplay.FontSize);
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LPTED.SavePage(NowPage);//先保存

                LPTED.OADisplay.FontFamily = new FontFamily(fd.Font.FontFamily.Name);//储存设置
                LPTED.OADisplay.FontSize = fd.Font.Size;

                ButtonOAFontFamily.Content = "字体:" + fd.Font.FontFamily.Name;
                LPTED.DisplaySource(NowPage);//重新加载
            }
            fd.Dispose();
        }

        private void ComboBoxOAFontSize_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeOAFontSize();
        }

        private void ComboBoxOAFontSize_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ChangeOAFontSize();
        }

        private void ComboBoxOAFontSize_DropDownClosed(object sender, EventArgs e)
        {
            ChangeOAFontSize();
        }
        private void ChangeOAFontSize()
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

            LPTED.EachPage.Insert(NowPage, new LinePutScript.LpsDocument());
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
                MessageBox.Show("最后一页面无法删除", "删除失败");
                return;
            }
            if (MessageBox.Show("确认删除该页面吗,此操作无法撤回", "删除本页面", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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

        private void ButtonFontFamily_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox1.Selection.IsEmpty)
                return;
            FontDialog fd = new FontDialog();
            if (float.TryParse(ComboBoxFontSize.Text, out float res))
                fd.Font = new Font(((string)ButtonFontFamily.Content).Substring(3), res);
            else
                fd.Font = new Font(((string)ButtonFontFamily.Content).Substring(3), LPTED.OADisplay.FontSize);
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TextBox1.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, fd.Font.FontFamily.Name);
                TextBox1.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fd.Font.Size.ToString("f1"));

                ButtonFontFamily.Content = "字体:" + fd.Font.FontFamily.Name;
                ComboBoxFontSize.Text = fd.Font.Size.ToString("f1");
            }
            fd.Dispose();
        }


        //每次选定的时候更新编辑页面的功能

        private void TextBox1_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (TextBox1.Selection.IsEmpty)
                return;
            TextRange tmp = new TextRange(TextBox1.Selection.Start, TextBox1.Selection.Start.GetNextContextPosition(LogicalDirection.Backward));
            ComboBoxFontSize.Text = tmp.GetPropertyValue(TextElement.FontSizeProperty).ToString();
            ButtonFontFamily.Content = "字体:" + tmp.GetPropertyValue(TextElement.FontFamilyProperty).ToString();
        }


        private void ComboBoxFontSize_DropDownClosed(object sender, EventArgs e)
        {
            ChangeFontSize();
        }

        private void ComboBoxFontSize_LostFocus(object sender, RoutedEventArgs e)
        {
            ChangeFontSize();
        }

        private void ComboBoxFontSize_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            ChangeFontSize();
        }

        private void ChangeFontSize()
        {
            if (!float.TryParse(ComboBoxFontSize.Text, out float fsize))
            {
                return;
            }
            TextBox1.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, fsize.ToString("f1"));
        }

        private void ButtonBold_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox1.Selection.GetPropertyValue(TextElement.FontWeightProperty).ToString() != "Bold")
            {
                TextBox1.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, "Bold");
            }
            else
                TextBox1.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, "Normal");
        }

        private void ButtonItalic_Click(object sender, RoutedEventArgs e)
        {
            if (TextBox1.Selection.GetPropertyValue(TextElement.FontStyleProperty).ToString() != "Italic")
            {
                TextBox1.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, "Italic");
            }
            else
                TextBox1.Selection.ApplyPropertyValue(TextElement.FontStyleProperty, "Normal");
        }

        private void ButtonUnderline_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((TextDecorationCollection)TextBox1.Selection.GetPropertyValue(Inline.TextDecorationsProperty)).Count == 0 ||
                    ((TextDecorationCollection)TextBox1.Selection.GetPropertyValue(Inline.TextDecorationsProperty))[0].Location != TextDecorationLocation.Underline)
                {
                    TextBox1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, "Underline");
                }
                else
                    TextBox1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
            catch
            {
                TextBox1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, "Underline");
            }
        }

        private void ButtonStrikethrough_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (((TextDecorationCollection)TextBox1.Selection.GetPropertyValue(Inline.TextDecorationsProperty)).Count == 0 ||
                    ((TextDecorationCollection)TextBox1.Selection.GetPropertyValue(Inline.TextDecorationsProperty))[0].Location != TextDecorationLocation.Strikethrough)
                {
                    TextBox1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, "Strikethrough");
                }
                else
                    TextBox1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
            }
            catch
            {
                TextBox1.Selection.ApplyPropertyValue(Inline.TextDecorationsProperty, "Strikethrough");
            }
        }
        public void ChangeAlignment(TextAlignment TA)
        {
            var tp = TextBox1.Selection.Start;
            foreach (Block block in TextBox1.Document.Blocks)
            {
                if (block.ElementStart.CompareTo(tp) == -1 && block.ElementEnd.CompareTo(tp) == 1)
                {
                    block.TextAlignment = TA;
                    return;
                }
            }
        }

        private void ButtonAjustify_Click(object sender, RoutedEventArgs e)
        {
            ChangeAlignment(TextAlignment.Justify);
        }

        private void ButtonAcenter_Click(object sender, RoutedEventArgs e)
        {
            ChangeAlignment(TextAlignment.Center);
        }

        private void ButtonAleft_Click(object sender, RoutedEventArgs e)
        {
            ChangeAlignment(TextAlignment.Left);
        }

        private void ButtonAright_Click(object sender, RoutedEventArgs e)
        {
            ChangeAlignment(TextAlignment.Right);
        }

        private void ButtonClearFormat_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Selection.ClearAllProperties();
        }

        private void ButtonSizebig_Click(object sender, RoutedEventArgs e)
        {
            double size;
            if (TextBox1.Selection.GetPropertyValue(TextElement.FontSizeProperty).GetType() == typeof(Double))
            {
                size = (double)TextBox1.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            }
            else
            {
                TextRange tmp = new TextRange(TextBox1.Selection.Start, TextBox1.Selection.Start.GetNextContextPosition(LogicalDirection.Backward));
                size = (double)tmp.GetPropertyValue(TextElement.FontSizeProperty); ;
            }
            TextBox1.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (size * 1.25).ToString("f1"));
        }

        private void ButtonSizesmall_Click(object sender, RoutedEventArgs e)
        {
            double size;
            if (TextBox1.Selection.GetPropertyValue(TextElement.FontSizeProperty).GetType() == typeof(Double))
            {
                size = (double)TextBox1.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            }
            else
            {
                TextRange tmp = new TextRange(TextBox1.Selection.Start, TextBox1.Selection.Start.GetNextContextPosition(LogicalDirection.Backward));
                size = (double)tmp.GetPropertyValue(TextElement.FontSizeProperty); ;
            }
            TextBox1.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (size * 0.8).ToString("f1"));
        }

        private void ButtonFontColor_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, ButtonCGFontColor.Background);
        }

        private void ButtonBackColor_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, ButtonCGBackColor.Background);
        }

        private void ButtonCGFontColor_Click(object sender, RoutedEventArgs e)
        {
            ButtonCGFontColor.IsChecked = false;
            ColorDialog cd = new ColorDialog();
            cd.CustomColors = CustomColors;
            cd.Color = ColorConvent(((SolidColorBrush)ButtonCGFontColor.Background).Color);
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ButtonCGFontColor.Background = new SolidColorBrush(ColorConvent(cd.Color));
            }
            //储存自定义颜色
            CustomColors = cd.CustomColors;
            cd.Dispose();
            TextBox1.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, ButtonCGFontColor.Background);
        }

        private void ButtonCGBackColor_Click(object sender, RoutedEventArgs e)
        {
            ButtonCGBackColor.IsChecked = false;
            ColorDialog cd = new ColorDialog();
            cd.CustomColors = CustomColors;
            cd.Color = ColorConvent(((SolidColorBrush)ButtonCGBackColor.Background).Color);
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ButtonCGBackColor.Background = new SolidColorBrush(ColorConvent(cd.Color));
            }
            //储存自定义颜色
            CustomColors = cd.CustomColors;
            cd.Dispose();
            TextBox1.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, ButtonCGBackColor.Background);
        }

        //Todo:IA的应用于更改(在切换栏
    }
}
