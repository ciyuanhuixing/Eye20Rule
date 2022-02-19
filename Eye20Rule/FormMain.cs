using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Eye20Rule
{
    public partial class FormMain : Form
    {
        private System.Timers.Timer timer;
        private int second;
        private int minute;

        public FormMain()
        {
            InitializeComponent();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
            this.Icon = Properties.Resources.logo;
            notifyIcon1.Icon = Properties.Resources.logo;
            label1.Image = Properties.Resources.logo.ToBitmap();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Timer_Elapsed(null, null);
            timer.Start();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                labelTime.Text = minute.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0');

                if (minute >= 20)
                {
                    timer.Stop();
                    Form formP = new FormPopUp();
                    formP.FormClosed += FormP_FormClosed;
                    formP.Show();
                }
            }));

            second++;
            if (second >= 60)
            {
                second = 0;
                minute++;
            }
        }

        private void FormP_FormClosed(object sender, FormClosedEventArgs e)
        {
            second = 0;
            minute = 0;
            timer.Start();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            this.Visible = true;
            this.Activate();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Dispose();
            notifyIcon1.Dispose();
            Environment.Exit(0);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        private void FormMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
