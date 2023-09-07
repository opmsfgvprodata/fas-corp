using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_Pkt
    {
        [Key]
        public int fld_PktID { get; set; }

        public string fld_PktKod { get; set; }

        public string fld_PktNama { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }

        public int fld_DivisionID { get; set; }

        public byte fld_JenisPktKod { get; set; }

        public string fld_JnsLot { get; set; }
    }
}