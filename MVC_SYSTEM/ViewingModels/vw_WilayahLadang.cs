using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ViewingModels 
{
    public class vw_WilayahLadang
    {
        
        public int? fld_ID { get; set; }
        public string LdgCode { get; set; }
        public string LdgName { get; set; }
        public int? WlyhID { get; set; }
        public string LdgEmail { get; set; }
        public bool? Deleted { get; set; }
        public string NoAcc { get; set; }
        public string NoGL { get; set; }
        public string NoCIT { get; set; }
        public string WilayahName { get; set; }
    }
}