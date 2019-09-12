using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace LineputPlus
{
    /// <summary>
    /// Lineputxaml.xaml 的交互逻辑
    /// </summary>
    public partial class Lineputxaml : Window
    {
        readonly LPTPlayer LPTPY;
        /// <summary>
        /// 什么内容都不带直接启动//理论上不可能 仅用于调试
        /// </summary>
        [Obsolete]
        public Lineputxaml()
        {
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
        }
        /// <summary>
        /// 带内容的启动
        /// </summary>
        /// <param name="lps">文档</param>
        public Lineputxaml(string LPT, int start = 1)
        {//ToDo:展示
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            LPTPY = new LPTPlayer(TextBox1, LPT, start);

            TextBox1.Document.Blocks.Clear();

            TextBox1.Background = new SolidColorBrush(LPTPY.OADisplay.BackColor);

            TextBox1.Document.Foreground = new SolidColorBrush(LPTPY.OADisplay.FontColor);
            TextBox1.Document.Background = new SolidColorBrush(LPTPY.OADisplay.BackColor);

            LPTPY.Next();
        }
        private void ButtonColorChoose_Click(object sender, RoutedEventArgs e)
        {
            BorderDisplayColor.Background = ((Button)sender).Background;
        }

        private void ButtonFontColor_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, BorderDisplayColor.Background);
        }

        private void ButtonBackColor_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, BorderDisplayColor.Background);
        }

        private void ButtonB_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextElement(TextElement.FontWeightProperty, "Bold", "Normal");
        }

        private void ButtonU_Click(object sender, RoutedEventArgs e)
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

        private void ButtonT_Click(object sender, RoutedEventArgs e)
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

        private void ButtonI_Click(object sender, RoutedEventArgs e)
        {
            ChangeTextElement(TextElement.FontStyleProperty, "Italic", "Normal");
        }
        private void ChangeTextElement(DependencyProperty TextElement, string Element, string Normal)
        {
            if (TextBox1.Selection.GetPropertyValue(TextElement).ToString() != Element)
            {
                TextBox1.Selection.ApplyPropertyValue(TextElement, Element);
            }
            else
                TextBox1.Selection.ApplyPropertyValue(TextElement, Normal);
        }

        private void ButtonSizeSmall_Click(object sender, RoutedEventArgs e)
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

        private void ButtonSizeBig_Click(object sender, RoutedEventArgs e)
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

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            LPTPY.Next();
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            LPTPY.Back();
        }
    }
}
