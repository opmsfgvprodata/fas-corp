using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsSAPPUP
{
    public class UploadResult
    {
        public string TYPE { get; set; }

        public string ID { get; set; }

        public string NUMBER { get; set; }

        public string MESSAGE { get; set; }

        public string LOG_NO { get; set; }

        public string LOG_MSG_NO { get; set; }

        public string MESSAGE_V1 { get; set; }

        public string MESSAGE_V2 { get; set; }

        public string MESSAGE_V3 { get; set; }

        public string MESSAGE_V4 { get; set; }

        public string PARAMETER { get; set; }

        public string ROW { get; set; }

        public string FIELD { get; set; }

        public string SYSTEM { get; set; }
    }
}