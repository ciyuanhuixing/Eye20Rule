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
        private int num;
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
            this.Icon = Properties.Resources.logo;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            const string tip = "您已持续用眼20分钟，休息一会吧！请向至少6米远处的物体眺望至少20秒，全神贯注凝视远处物体并辨认其轮廓。";

            this.Invoke(new MethodInvoker(() =>
            {
                if (num > 20)
                {
                    labelTime.Text = tip;
                }
                else if (num >= 0)
                {
                    labelTime.Text = tip + "\r\n\r\n倒计时：" + num;
                }
                else
                {
                    timer.Enabled = false;
                    timer.Stop();
                    Close();
                }
                num--;
            }));
        }

        private void FormPopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            Visible = false;
            e.Cancel = true;
        }

        private void FormPopUp_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                num = 30;
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

        private void FormPopUp_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
