namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_RptMasterDataPkj
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(30)]
        public string fld_NamaWilayah { get; set; }

        [StringLength(5)]
        public string fld_KodLadang { get; set; }

        [StringLength(20)]
        public string fld_NoPkj { get; set; }

        [StringLength(50)]
        public string fld_Nama { get; set; }

        [StringLength(20)]
        public string fld_NoKP { get; set; }

        [StringLength(10)]
        public string fld_KodWarganegara { get; set; }

        [StringLength(10)]
        public string fld_KodKategoriPekerja { get; set; }

        [StringLength(10)]
        public string fld_KodPembekal { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhMulaKhidmat { get; set; }

        [StringLength(10)]
        public string fld_KodStatusAktif { get; set; }

        [StringLength(10)]
        public string fld_KodSebabTakAktif { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhTamatPermit { get; set; }

        [Column(TypeName = "date")]
        public DateTime? fld_TarikhTamatPassport { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }
    }
}
