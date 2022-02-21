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
            Width = Screen.GetBounds(this).Width;
            //BackColor = Color.FromArgb(179, 235, 181);
        }

        private void FormPopUp_Load(object sender, EventArgs e)
        {
            Timer_Elapsed(null, null);
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (num > 0)
                {
                    labelTime.Text = "您已持续用眼20分钟，休息一会吧！请向至少6米远处眺望至少20秒（点此可跳过本次休息）。20秒倒计时：" + num;
                }
                else
                {
                    timer.Enabled = false;
                    BackColor = Color.Black;
                    labelTime.Text = "20秒已过，若已休息完毕，请点此重新开始20分钟的计时。";
                }
                num--;
            }));
        }

        private void labelTime_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormPopUp_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer.Dispose();
        }
    }
}
