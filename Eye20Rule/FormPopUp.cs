using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Eye20Rule
{
    public partial class FormPopUp : Form
    {
        private int num = 20;
        private System.Timers.Timer timer;

        public FormPopUp()
        {
            InitializeComponent();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
        }

        private void FormPopUp_Load(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                labelTime.Text = "请向至少6米远处眺望至少20秒。\r\n\r\n20秒倒计时：\r\n\r\n" + num;
                if (num <= 0)
                {
                    label1.Visible = false;
                    labelTime.Text = "请按Esc键或点任意处关闭此遮罩";
                }
                num--;
            }));
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormPopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Dispose();
            timer = null;
        }

        private void labelTime_Click(object sender, EventArgs e)
        {
            if (num > 0)
            {
                return;
            }
            this.Close();
        }

        private void FormPopUp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
