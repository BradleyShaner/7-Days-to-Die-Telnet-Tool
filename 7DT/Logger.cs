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
        static TextBox logOutput;

        static public void SetUpLogOutput(TextBox log)
        {
            logOutput = log;
        }


        static public void AddLog(string text)
        {
            if (logOutput.InvokeRequired)
            {
                logOutput.BeginInvoke((MethodInvoker)delegate () { logOutput.AppendText(text + "\n"); ; });
            }
            else
            {
                logOutput.AppendText(text + "\n");
            }

            /*
            try
            {
                logOutput.AppendText(text + "\n");

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Logger error");
            }
            */

        }

    }
}
