using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLibrary;

namespace EchoSync
{
    static class Program
    {
        //static int x;
        //private static BackgroundWorker mainBackgroundWorker;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.Init("C:\\Temp\\application.log");
            Logger.Log("first log message");
            smallFunc();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Form1 f = new Form1();
            Application.Run(f);
            Console.Out.WriteLine("line");
            f.textBox1.Text = "test string 1";
        }

        static public void smallFunc()
        {
            Logger.Log("");
            TestLibInterfaceClient client = new TestLibInterfaceClient();
            string s = client.GetData(4);
            Logger.Log(s);
        }
    }
}
