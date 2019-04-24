using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lineput
{
    public partial class FrmLPP : Form
    {
        public FrmLPP()
        {
            InitializeComponent();
        }

        private void PictureBox1_MouseHover(object sender, EventArgs e)
        {
            MenuStrip1.Visible = false;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            MenuStrip1.Visible = true;
        }

        private void PictureBox1_MouseLeave(object sender, EventArgs e)
        {
            MenuStrip1.Visible = false;
        }
    }
}
