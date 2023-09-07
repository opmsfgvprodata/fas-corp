namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_RptRmsnPenPkjLdg
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_NamaWilayah { get; set; }

        [StringLength(5)]
        public string fld_KodLadang { get; set; }

        [StringLength(50)]
        public string fld_NamaLadang { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        public int? fld_JumBilPekerjaL { get; set; }

        public int? fld_JumBilPekerjaS { get; set; }

        public int? fld_JumPkjPen1000Kbwh { get; set; }

        public int? fld_JumPkjPen1000KbwhS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumPkjPen1000KbwhPrcnt { get; set; }

        public int? fld_JumPkjPen10011500 { get; set; }

        public int? fld_JumPkjPen10011500S { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumPkjPen10011500Prcnt { get; set; }

        public int? fld_JumPkjPen1501Kats { get; set; }

        public int? fld_JumPkjPen1501KatsS { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumPkjPen1501KatsPrcnt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumPkjPen1000KbwhInd { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumPkjPen10011500Ind { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumPkjPen1501KatsInd { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PrtaPenTngi { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_PrtaPenRndh { get; set; }

        [StringLength(2)]
        public string fld_WargaNegara { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }
    }
}
