using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_JenisPkt
    {
        [Key]
        public int fld_JenisPktID { get; set; }

        public byte fld_JenisPktKod { get; set; }

        public string fld_JenisPktNama { get; set; }
    }
}