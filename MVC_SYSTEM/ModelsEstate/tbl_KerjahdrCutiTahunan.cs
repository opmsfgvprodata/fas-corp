namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KerjahdrCutiTahunan
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_Nopkj { get; set; }

        [StringLength(50)]
        public string fld_Kum { get; set; }

        [StringLength(50)]
        public string fld_KodCuti { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_Kadar { get; set; }

        public int? fld_JumlahCuti { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_JumlahAmt { get; set; }

        public bool? fld_StatusAmbil { get; set; }

        public int? fld_Month { get; set; }

        public int? fld_Year { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }
    }
}
