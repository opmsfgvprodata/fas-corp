using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsSAPPUP
{
    public class SAPPUPMessage
    {
        public string SuccessCode()
        {
            var message = "S";

            return message;
        }

        public string ErrorCode()
        {
            var message = "E";

            return message;
        }

        public string SuccessMessage()
        {
            var message = "Success";

            return message;
        }

        public string ErrorMessage()
        {
            var message = "Error";

            return message;
        }

        public string SystemName()
        {
            var message = "OPMS";

            return message;
        }

        public string CreateDataSuccessMessage(int count)
        {
            var message = count + " record(s) created successfully";

            return message;
        }

        public string UpdateDataSuccessMessage(int count)
        {
            var message = count + " record(s) updated successfully";

            return message;
        }
    }
}