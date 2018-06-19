using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using WMS_Client.UI;

namespace WMS_Client
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (IsRunning())
            {
                MessageBox.Show("程序已经打开！");
                return;
            }

            Application.Run(new MFrm());
        }

        public static bool IsRunning()
        {
            Process current = default(Process);
            current = Process.GetCurrentProcess();
            Process[] processes = null;
            processes = Process.GetProcessesByName(current.ProcessName);

            Process process = default(Process);

            foreach (Process tempLoopVar_process in processes)
            {
                process = tempLoopVar_process;

                if (process.Id != current.Id)
                {
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
