using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharedLibrary;

namespace EchoSync
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void getStatusButton_Click(object sender, EventArgs e)
        {
            TestLibInterfaceClient client = new TestLibInterfaceClient();
            string s = client.GetData(5);
            Logger.Log(s);
            textBox1.Text += s + "\r\n";
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void getTimeButton_Click(object sender, EventArgs e)
        {
            TestLibInterfaceClient client = new TestLibInterfaceClient();
            long s = client.GetTime();
            Logger.Log("" + s);
            textBox1.Text += "Time: " + s + "\r\n";
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }
    }
}
