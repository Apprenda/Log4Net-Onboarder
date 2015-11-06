using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using log4net;

namespace Wcf_Standalone_Attribute
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        public Service1()
        {
            var logger = LogManager.GetLogger(GetType());
            logger.Warn("Logging from WCF-Standalone-Explicit");
            logger.Warn(
                "Starting sample workload configured with XmlConfigurator.ConfigureAndWatch for default.log4net. (At WARN)");
            logger.Info("Sample log at INFO.");
            logger.Debug("Sample log at DEBUG");
            logger.Error("Sample log at ERROR");
            logger.Fatal("Sample log at FATAL");
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
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
