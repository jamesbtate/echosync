using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SharedLibrary;

namespace TestService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TestServ" in both code and config file together.
    public class TestLib : TestLibInterface
    {
        public string GetData(int value)
        {
            Logger.Debug2();
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            Logger.Debug2();
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
