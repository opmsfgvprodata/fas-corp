namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_RptDdkPrluPkjLdg
    {
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_NamaWilayah { get; set; }

        [StringLength(5)]
        public string fld_KodLadang { get; set; }

        [StringLength(50)]
        public string fld_NamaLadang { get; set; }

        public int? fld_JumLdg { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasLadang { get; set; }

        public int? fld_PerluThn { get; set; }

        public int? fld_PerluSmsa { get; set; }

        public int? fld_Nisbah { get; set; }

        public int? fld_TKT { get; set; }

        public int? fld_TKI { get; set; }

        public int? fld_TKB { get; set; }

        public int? fld_TKN { get; set; }

        public int? fld_TKIN { get; set; }

        public int? fld_TKP { get; set; }

        public int? fld_TKK { get; set; }

        public int? fld_TKTH { get; set; }

        public int? fld_TKV { get; set; }

        public int? fld_JumPkrja { get; set; }

        public int? fld_TKTS { get; set; }

        public int? fld_TKIS { get; set; }

        public int? fld_TKBS { get; set; }

        public int? fld_TKNS { get; set; }

        public int? fld_TKINS { get; set; }

        public int? fld_TKPS { get; set; }

        public int? fld_TKKS { get; set; }

        public int? fld_TKTHS { get; set; }

        public int? fld_TKVS { get; set; }

        public int? fld_JumPkrjaS { get; set; }

        public int? fld_KekurangSmsa { get; set; }

        public int? fld_BilPkjSmsPemandu { get; set; }

        public int? fld_BilPkjSmsPenoreh { get; set; }

        public int? fld_BilPkjSmsPenuai { get; set; }

        public int? fld_BilPkjSmsPekerjaAm { get; set; }

        public int? fld_BilPkjSmsOpOperasi { get; set; }

        public int? fld_BilPkjSmsMandorAm { get; set; }

        public int? fld_BilPkjSmsLoader { get; set; }

        public int? fld_BilPkjSmsInpekKualiti { get; set; }

        public int? fld_BilPkjSmsSemaian { get; set; }

        public int? fld_JumBilPkjSms { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public int? fld_CreatedBy { get; set; }
    }
}
