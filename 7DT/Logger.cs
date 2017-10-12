using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _7DT
{
    
    static class Logger
    {
        static RichTextBox logOutput;
        private static object _logLock = new object();

        static public void SetUpLogOutput(RichTextBox log)
        {
            logOutput = log;
        }


        static public void AddLog(string text)
        {
            lock (_logLock)
            {
                if (logOutput.InvokeRequired)
                {
                    logOutput.BeginInvoke((MethodInvoker)delegate () { logOutput.AppendText(text + "\n"); ; });
                }
                else
                {
                    logOutput.AppendText(text + "\n");
                }
            }
        }
    }
}
