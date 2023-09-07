using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI
{
    public class WorkData
    {
        [Key]
        public int fld_WorkID { get; set; }

        public string fld_Nopkj { get; set; }

        public string fld_KodKumpulan { get; set; }

        public DateTime fld_Tarikh { get; set; }

        public string fld_LajerKod { get; set; }

        public byte fld_JenisPktKod { get; set; }

        public string fld_PktKod { get; set; }

        public string fld_ActivityTypeCode { get; set; }

        public string fld_Unit { get; set; }

        public decimal fld_Rate { get; set; }

        public decimal fld_Hasil { get; set; }

        public decimal fld_Amount { get; set; }

        public byte fld_Bonus { get; set; }

        public short fld_Quality { get; set; }

        public decimal fld_OT { get; set; }

        public string fld_KodActivityOPMS { get; set; }

        public string fld_ActivitySAP { get; set; }

        public string fld_KodGL { get; set; }

        public string fld_CCNNCode { get; set; }

        public short fld_FormFlag { get; set; }

        public short fld_FormatForm { get; set; }

        public string fld_DataSource { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }

        public string fld_CreatedBy { get; set; }

        public DateTime fld_CreatedDT { get; set; }
    }
}