using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLibrary;

namespace EchoSync
{
    static class Program
    {
        static int x;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.Init();
            Logger.Log("first log message");
            smallFunc();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static public void smallFunc()
        {
            Logger.Log("");
            x = 5 + 4;
            x--;
        }
    }
}
