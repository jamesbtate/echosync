using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using SharedLibrary;

namespace TestService
{
    /// <summary>
    /// A Windows Service
    /// </summary>
    public partial class TestServ : ServiceBase
    {
        public ServiceHost serviceHost = null;

        public TestServ()
        {
            InitializeComponent();
            ServiceName = "EchoSyncTest";
        }

        protected override void OnStart(string[] args)
        {
            Logger.Init();
            Logger.Log("");
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            serviceHost = new ServiceHost(typeof(TestLib));
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }
}
