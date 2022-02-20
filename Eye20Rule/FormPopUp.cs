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
        private System.Timers.Timer timer;

        public FormPopUp()
        {
            InitializeComponent();
            labelTime.BackColor = Color.FromArgb(179, 235, 181);
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            SetFormToolWindowStyle(this);
        }

        public void StartTimer()
        {
            num = 20;
            Timer_Elapsed(null, null);
            Opacity = 100;
            timer.Enabled = true;
            timer.Start();
        }

        public void StopTimer()
        {
            Opacity = 0;
            timer.Enabled = false;
            timer.Stop();
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (num > 0)
                {
                    labelTime.Text = "您已持续用眼20分钟，休息一会吧！请向至少6米远处眺望至少20秒（点此文字可跳过休息）。20秒倒计时：" + num;
                }
                else
                {
                    labelTime.Text = "已过了20秒，您休息结束时请点此重新开始20分钟的计时。";
                }
                num--;
            }));
        }

        private void labelTime_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        [DllImport("user32.dll")]
        public static extern Int32 GetWindowLong(IntPtr hwnd, Int32 index);

        [DllImport("user32.dll")]
        public static extern Int32 SetWindowLong(IntPtr hwnd, Int32 index, Int32 newValue);

        public const int GWL_EXSTYLE = (-20);

        public static void AddWindowExStyle(IntPtr hwnd, Int32 val)
        {
            int oldValue = GetWindowLong(hwnd, GWL_EXSTYLE);
            if (oldValue == 0)
            {
                throw new System.ComponentModel.Win32Exception();
            }
            if (0 == SetWindowLong(hwnd, GWL_EXSTYLE, oldValue | val))
            {
                throw new System.ComponentModel.Win32Exception();
            }
        }

        public static int WS_EX_TOOLWINDOW = 0x00000080;

        /// <summary>
        /// 让窗体不会在ALT+TAB中出现
        /// </summary>
        /// <param name="form"></param>
        public static void SetFormToolWindowStyle(System.Windows.Forms.Form form)
        {
            AddWindowExStyle(form.Handle, WS_EX_TOOLWINDOW);
        }
    }
}
