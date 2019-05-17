using LinePutScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LineputPlus
{
    /// <summary>
    /// Lineputxaml.xaml 的交互逻辑
    /// </summary>
    public partial class Lineputxaml : Window
    {
        /// <summary>
        /// 什么内容都不带直接启动
        /// </summary>
        public Lineputxaml()
        {
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
        }
        /// <summary>
        /// 带内容的启动
        /// </summary>
        /// <param name="lps">文档</param>
        public Lineputxaml(LpsDocument lps)
        {//ToDo:展示
            this.WindowState = WindowState.Maximized;
            InitializeComponent();
        }
        string ChooseColor = "White";
        private void ButtonColorChoose_Click(object sender, RoutedEventArgs e)
        {
            ChooseColor = ((Button)sender).Name.Substring(6);
            Color Choose = Colors.White;
            switch (ChooseColor)
            {
                case "Red":
                    Choose = Colors.Red;
                    break;
                case "Lime":
                    Choose = Colors.Lime;
                    break;
                case "Blue":
                    Choose = Colors.Blue;
                    break;
                case "Orange":
                    Choose = Colors.Orange;
                    break;
                case "White":
                    Choose = Colors.White;
                    break;
                case "Black":
                    Choose = Colors.Black;
                    break;
            }
            BorderDisplayColor.Background = new SolidColorBrush(Choose);
        }

        private void ButtonFontColor_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Selection.ApplyPropertyValue(TextElement.ForegroundProperty, ChooseColor);
        }

        private void ButtonBackColor_Click(object sender, RoutedEventArgs e)
        {
            TextBox1.Selection.ApplyPropertyValue(TextElement.BackgroundProperty, ChooseColor);
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
            if (TextBox1.Selection.GetPropertyValue(TextElement.FontSizeProperty).GetType()== typeof(Double))
            {
                size = (double)TextBox1.Selection.GetPropertyValue(TextElement.FontSizeProperty);
            }
            else
            {
                TextRange tmp = new TextRange(TextBox1.Selection.Start, TextBox1.Selection.Start.GetNextContextPosition(LogicalDirection.Backward));
                MessageBox.Show(Convert.ToString(tmp.GetPropertyValue(TextElement.FontSizeProperty)));
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
                MessageBox.Show(Convert.ToString(tmp.GetPropertyValue(TextElement.FontSizeProperty)));
                size = (double)tmp.GetPropertyValue(TextElement.FontSizeProperty); ;
            }
            TextBox1.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, (size * 1.25).ToString("f1"));
        }
    }
}
