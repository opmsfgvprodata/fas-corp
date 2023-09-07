using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI
{
    public class AttendanceResultUpload
    {
        public int AttReturnID { get; set; }

        public string fld_Nopkj { get; set; }

        public DateTime fld_Tarikh { get; set; }

        public short fld_Status { get; set; }

        public string Msg { get; set; }
    }
}