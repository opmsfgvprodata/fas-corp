namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_RptMklmtPkjTmptn
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_NamaWilayah { get; set; }

        [StringLength(50)]
        public string fld_KodLadang { get; set; }

        [StringLength(50)]
        public string fld_NamaLadang { get; set; }

        [StringLength(20)]
        public string fld_Nokp { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        public int? fld_BilPkj { get; set; }

        public int? fld_Umr60Up { get; set; }

        public int? fld_Umr5660 { get; set; }

        public int? fld_Umr5155 { get; set; }

        public int? fld_Umr4650 { get; set; }

        public int? fld_Umr4145 { get; set; }

        public int? fld_Umr3640 { get; set; }

        public int? fld_Umr3135 { get; set; }

        public int? fld_Umr2630 { get; set; }

        public int? fld_Umr2125 { get; set; }

        public int? fld_Umr20Bel { get; set; }

        public int? fld_UmrUnkwn { get; set; }

        public int? fld_Kdmt35Up { get; set; }

        public int? fld_Kdmt3135 { get; set; }

        public int? fld_Kdmt2630 { get; set; }

        public int? fld_Kdmt2125 { get; set; }

        public int? fld_Kdmt1620 { get; set; }

        public int? fld_Kdmt1115 { get; set; }

        public int? fld_Kdmt0610 { get; set; }

        public int? fld_Kdmt0105 { get; set; }

        public int? fld_Kdmt01Bel { get; set; }

        public int? fld_KdmtUnkwn { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }
    }
}
