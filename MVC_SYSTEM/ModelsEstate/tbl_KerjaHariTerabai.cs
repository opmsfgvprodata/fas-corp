namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_KerjaHariTerabai
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_JenisCharge { get; set; }

        [StringLength(10)]
        public string fld_MasaKerja { get; set; }

        public int? fld_ApprovedBy { get; set; }

        public DateTime? fld_ApprovedDT { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_Tarikh { get; set; }

        [StringLength(20)]
        public string fld_Nopkj { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
