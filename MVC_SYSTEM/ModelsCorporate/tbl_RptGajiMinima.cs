namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_RptGajiMinima
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

        public int? fld_BilanganTawaranHariBekerja { get; set; }

        public int? fld_BilanganHariBekerjaSebenar { get; set; }

        public decimal? fld_GajiBulanan { get; set; }

        [StringLength(10)]
        public string fld_SebabGajiMinima { get; set; }

        [StringLength(10)]
        public string fld_TindakanGajiMinima { get; set; }

        public int? fld_Bulan { get; set; }

        public int? fld_Tahun { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WIlayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }
    }
}
