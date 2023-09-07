using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_Lajer
    {
        [Key]
        public int fld_LajerID { get; set; }

        public string fld_LajerKod { get; set; }

        public string fld_Keterangan { get; set; }
    }
}