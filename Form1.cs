using System;
using System.Drawing;
using System.Windows.Forms;

namespace kusa
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.StartPosition = FormStartPosition.Manual;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 20; i++)
            {
                this.TransparencyKey = this.BackColor;
                frmBackground frmBackground = new frmBackground(Properties.Resources.kusa);
                Random random = new Random();
                var x = random.Next(-100, Screen.PrimaryScreen.Bounds.Width + 50);
                var y = Screen.PrimaryScreen.WorkingArea.Height - Properties.Resources.kusa.Height;
                this.Left = x;
                this.Top = y;
                Point point = new Point(x, y);
                this.Location = point;
                point.Offset(0, 0);
                frmBackground.Location = point;
                frmBackground.StartPosition = FormStartPosition.Manual;
                frmBackground.Show();
            }
        }
    }
}
