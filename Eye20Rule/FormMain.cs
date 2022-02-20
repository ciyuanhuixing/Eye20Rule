using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Eye20Rule
{
    public partial class FormMain : Form
    {
        private System.Timers.Timer timer;
        private int second;
        private int minute;
        FormPopUp formP = new FormPopUp();

        public FormMain()
        {
            InitializeComponent();
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += Timer_Elapsed;
            this.Icon = Properties.Resources.logo;
            notifyIcon1.Icon = Properties.Resources.logo;
            pictureBox1.BackgroundImage = Properties.Resources.logo.ToBitmap();

            string[] strArgs = Environment.GetCommandLineArgs();
            if (strArgs.Length >= 2 && strArgs[1] == "-silent")
            {
                Opacity = 0;
                ShowInTaskbar = false;
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            formP.FormClosing += FormP_FormClosing;
            formP.Show();
            CheckAutoRun();
            Timer_Elapsed(null, null);
            timer.Enabled = true;
        }

        private void FormP_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel= true;
            formP.StopTimer();

            second = 0;
            minute = 0;
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
                    formP.StartTimer();
                }
            }));

            second++;
            if (second >= 60)
            {
                second = 0;
                minute++;
            }
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
            try
            {
                SetMeAutoStart(!menuAutoRun.Checked);
                menuAutoRun.Checked = !menuAutoRun.Checked;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "错误提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 检查是否开机启动
        /// </summary>
        private void CheckAutoRun()
        {
            List<string> shortcutPaths = GetQuickFromFolder(startupPath, appTargetPath);
            if (shortcutPaths.Count > 0)
            {
                menuAutoRun.Checked = true;
            }
            else
            {
                menuAutoRun.Checked = false;
            }
        }

        /// <summary>
        /// 系统自动启动目录
        /// </summary>
        private string startupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        /// <summary>
        /// 程序快捷方式目标路径
        /// </summary>
        private string appTargetPath = Process.GetCurrentProcess().MainModule.FileName;

        /// <summary>
        /// 设置开机自动启动
        /// </summary>
        /// <param name="onOff">是否自动启动</param>
        public void SetMeAutoStart(bool onOff = true)
        {
            List<string> shortcutPaths = GetQuickFromFolder(startupPath, appTargetPath);

            if (onOff)
            {
                //存在2个以快捷方式则保留一个快捷方式-避免重复多于
                if (shortcutPaths.Count >= 2)
                {
                    for (int i = 1; i < shortcutPaths.Count; i++)
                    {
                        DeleteFile(shortcutPaths[i]);
                    }
                }
                else if (shortcutPaths.Count < 1)
                {
                    CreateShortcut(startupPath, Text, appTargetPath);
                }
            }
            else
            {
                if (shortcutPaths.Count > 0)
                {
                    for (int i = 0; i < shortcutPaths.Count; i++)
                    {
                        DeleteFile(shortcutPaths[i]);
                    }
                }
            }
        }

        /// <summary>
        ///  向目标路径创建指定文件的快捷方式
        /// </summary>
        /// <param name="directory">目标目录</param>
        /// <param name="shortcutName">快捷方式名字</param>
        /// <param name="targetPath">文件完全路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标地址</param>
        /// <returns>成功或失败</returns>
        private void CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            string shortcutPath = Path.Combine(directory, $"{shortcutName}.lnk");

            //添加引用 Com 中搜索 Windows Script Host Object Model
            WshShell shell = new IWshRuntimeLibrary.WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.Arguments = "-silent";
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.WindowStyle = 1;
            shortcut.Description = description;
            shortcut.IconLocation = string.IsNullOrEmpty(iconLocation) ? targetPath : iconLocation;
            shortcut.Save();
        }

        /// <summary>
        /// 获取指定文件夹下指定应用程序的快捷方式路径集合
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="appPath">应用程序路径</param>
        /// <returns>应用程序的快捷方式</returns>
        private List<string> GetQuickFromFolder(string directory, string appPath)
        {
            List<string> tempStrs = new List<string>();
            string tempStr = null;
            string[] files = Directory.GetFiles(directory, "*.lnk");
            if (files == null || files.Length < 1)
            {
                return tempStrs;
            }
            for (int i = 0; i < files.Length; i++)
            {
                tempStr = GetAppPathFromQuick(files[i]);
                if (tempStr == appPath)
                {
                    tempStrs.Add(files[i]);
                }
            }
            return tempStrs;
        }

        /// <summary>
        /// 获取快捷方式的目标文件路径-用于判断是否已经开启了自动启动
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <returns></returns>
        private string GetAppPathFromQuick(string shortcutPath)
        {
            if (System.IO.File.Exists(shortcutPath))
            {
                WshShell shell = new WshShell();
                IWshShortcut shortct = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                return shortct.TargetPath;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 根据路径删除文件-用于取消自启时从计算机自启目录删除程序的快捷方式
        /// </summary>
        /// <param name="path">路径</param>
        private void DeleteFile(string path)
        {
            FileAttributes attr = System.IO.File.GetAttributes(path);
            if (attr == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// 在桌面上创建快捷方式-如果需要可以调用
        /// </summary>
        /// <param name="desktopPath">桌面地址</param>
        /// <param name="appPath">应用路径</param>
        public void CreateDesktopQuick(string desktopPath = "", string quickName = "", string appPath = "")
        {
            List<string> shortcutPaths = GetQuickFromFolder(desktopPath, appPath);
            //如果没有则创建
            if (shortcutPaths.Count < 1)
            {
                CreateShortcut(desktopPath, quickName, appPath, "软件描述");
            }
        }

        private void FormMain_Shown(object sender, EventArgs e)
        {
            if (Opacity == 0)
            {
                Visible = false;
                Opacity = 100;
            }
        }
    }
}
