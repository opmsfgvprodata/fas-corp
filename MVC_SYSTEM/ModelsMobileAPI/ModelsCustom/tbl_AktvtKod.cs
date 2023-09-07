using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_AktvtKod
    {
        [Key]
        public int fld_AktvtKodID { get; set; }

        public string fld_AktvtKod { get; set; }

        public string fld_Keterangan { get; set; }

        public string fld_Unit { get; set; }

        public decimal? fld_MaxProduktiviti { get; set; }

        public string fld_JenisAktvtKod { get; set; }

        public short? fld_FormatForm { get; set; }

        public string fld_KdhMenuai { get; set; }

        public decimal fld_Rate { get; set; }
    }
}