using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Eye20Rule
{
    public partial class FormPopUp : Form
    {
        private int num = 20;
        private System.Timers.Timer timer = new System.Timers.Timer(1000);

        /// <summary>
        /// 让弹出窗体无焦点
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_EX_NOACTIVATE = 0x08000000;
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_NOACTIVATE;
                return cp;
            }
        }

        public FormPopUp()
        {
            InitializeComponent();
            timer.Elapsed += Timer_Elapsed;
            Rectangle bounds = Screen.GetBounds(this);
            Location = new Point(bounds.Width - Width, bounds.Height - Height);
            labelLogo.Image = Properties.Resources.logo.ToBitmap();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (num >= 0)
                {
                    labelTime.Text = "您已持续用眼20分钟，休息一会吧！请向至少6米远处眺望至少20秒。\r\n\r\n倒计时：" + num;
                }
                else
                {
                    timer.Enabled = false;
                    timer.Stop();
                    btnClose.Text = "休息完毕";
                }
                num--;
            }));
        }

        private void FormPopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FormPopUp_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                btnClose.Text = "跳过本次休息";
                num = 20;
                Timer_Elapsed(null, null);
                timer.Enabled = true;
                timer.Start();
                ShowInTaskbar = true;
            }
            else
            {
                timer.Enabled = false;
                timer.Stop();
                ShowInTaskbar = false;
            }
        }
    }
}
