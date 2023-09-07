using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_Division
    {
        [Key]
        public int fld_DivisionID { get; set; }
        
        public string fld_DivisionName { get; set; }
    }
}