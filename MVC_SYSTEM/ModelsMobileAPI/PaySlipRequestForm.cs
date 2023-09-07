using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI
{
    public class PaySlipRequestForm
    {
        public int Month { get; set; }

        public int Year { get; set; }

        public string NoPkj { get; set; }

        public string EstateCode { get; set; }
    }
}