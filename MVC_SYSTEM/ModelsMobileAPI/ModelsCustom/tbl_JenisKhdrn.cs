using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_JenisKhdrn
    {
        [Key]
        public int fld_JenisKhdrnID { get; set; }

        public string fld_JenisKhdrnKod { get; set; }

        public string fld_Keterangan { get; set; }

        public string fld_StatusKhdran { get; set; }

        public short fld_RateMultiple { get; set; }
    }
}