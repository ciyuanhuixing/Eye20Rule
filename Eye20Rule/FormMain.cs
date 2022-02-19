using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            this.Icon = Properties.Resources.logo;
            notifyIcon1.Icon = Properties.Resources.logo;
            pictureBox1.BackgroundImage = Properties.Resources.logo.ToBitmap();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            CheckAutoRun();
            Timer_Elapsed(null, null);
            timer.Enabled = true;
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                labelTime.Text = minute.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0');

                if (minute >= 20)
                {
                    timer.Enabled = false;
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
            timer.Enabled = true;
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

        private void 开机启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sFilePath = Application.ExecutablePath;
            string sFileName = Path.GetFileName(sFilePath);
            try
            {
                SystemHelper.SetAutoRun(sFilePath, sFileName, !menuAutoRun.Checked);
                menuAutoRun.Checked = !menuAutoRun.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 检查是否开机启动，并设置控件状态
        /// </summary>
        private void CheckAutoRun()
        {
            string strFilePath = Application.ExecutablePath;
            string strFileName = System.IO.Path.GetFileName(strFilePath);
            if (SystemHelper.IsAutoRun(strFilePath, strFileName))
            {
                menuAutoRun.Checked = true;
            }
            else
            {
                menuAutoRun.Checked = false;
            }
        }
    }

    public sealed class SystemHelper
    {
        private SystemHelper() { }
        /// <summary>
        /// 设置程序开机启动
        /// </summary>
        /// <param name="sAppPath">应用程序exe所在文件夹</param>
        /// <param name="sAppName">应用程序exe名称</param>
        /// <param name="isAutoRun">自动运行状态</param>
        public static void SetAutoRun(string sAppPath, string sAppName, bool isAutoRun)
        {
            if (string.IsNullOrEmpty(sAppPath) || string.IsNullOrEmpty(sAppName))
            {
                throw new Exception("应用程序路径或名称为空！");
            }
            RegistryKey reg = Registry.LocalMachine;
            RegistryKey run = reg.CreateSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\");
            if (isAutoRun)
            {
                run.SetValue(sAppName, sAppPath);
            }
            else
            {
                if (null != run.GetValue(sAppName))
                {
                    run.DeleteValue(sAppName);
                }
            }
            run.Close();
            reg.Close();
        }

        /// <summary>
        /// 判断是否开机启动
        /// </summary>
        /// <param name="strAppPath">应用程序路径</param>
        /// <param name="strAppName">应用程序名称</param>
        /// <returns></returns>
        public static bool IsAutoRun(string strAppPath, string strAppName)
        {
            RegistryKey reg = Registry.LocalMachine;
            RegistryKey software = reg.OpenSubKey(@"SOFTWARE");
            RegistryKey run = reg.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\");
            object key = run.GetValue(strAppName);
            software.Close();
            run.Close();
            if (null == key || !strAppPath.Equals(key.ToString()))
            {
                return false;
            }
            return true;
        }
    }
}
