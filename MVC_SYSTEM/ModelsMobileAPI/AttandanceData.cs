using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI
{
    public class AttandanceData
    {
        [Key]
        public int fld_AttandanceID { get; set; }

        public string fld_Nopkj { get; set; }

        public string fld_KodKumpulan { get; set; }

        public DateTime fld_Tarikh { get; set; }

        public int fld_Hujan { get; set; }

        public string fld_Kdhdct { get; set; }

        public string fld_DataSource { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }

        public string fld_CreatedBy { get; set; }

        public DateTime fld_CreatedDT { get; set; }
    }
}