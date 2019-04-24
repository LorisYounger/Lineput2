using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineputNew
{

    static class Program
    {
        /// <summary>
        /// 应用程序启动参数。
        /// </summary>
        public static string[] args;

        public static readonly double Verizon = 2.01;
        public static readonly DateTime PublishTime = new DateTime(2018, 6, 12);
        public static readonly string Symbol = "NewBorn";

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Program.args = args;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //string abc = "";
            //foreach(string arg in args)
            //{
            //    abc += arg+"+";
            //}
            //MessageBox.Show(abc);
#if !DEBUG
            try
            {
#endif
                Application.Run(new FrmMain());
#if !DEBUG
            }
            catch (Exception ex)
            {
                MessageBox.Show($"LinePut {String.Format("{0:N2}", Program.Verizon)} 发生了严重错误,已经停止运行\n\n发生在:{ex.Source}\n错误信息:{ex.Message}\n{ex.Data.ToString()}", $"严重错误{{{ex.HResult}}}", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
#endif
        }

    }

}
