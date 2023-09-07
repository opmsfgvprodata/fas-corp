namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_JenisInsentif
    {
        [Key]
        public int fld_JenisInsentifID { get; set; }

        [StringLength(10)]
        public string fld_JenisInsentif { get; set; }

        [StringLength(5)]
        public string fld_KodInsentif { get; set; }

        [StringLength(50)]
        public string fld_Keterangan { get; set; }

        public decimal? fld_MinValue { get; set; }

        public decimal? fld_MaxValue { get; set; }

        public decimal? fld_FixedValue { get; set; }

        public decimal? fld_DailyFixedValue { get; set; }

        public bool? fld_AdaCaruman { get; set; }

        public int? fld_TetapanNilai { get; set; }

        public int? fld_KelayakanKepada { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_NegaraID { get; set; }

        [StringLength(5)]
        public string fld_KodAktvt { get; set; }

        //Commented by Shazana on 20/10
        //[StringLength(5)]
        //Added by Shazana on 20/10
        [StringLength(10)]
        public string fld_KodGL { get; set; }

        public bool? fld_Deleted { get; set; }

        //added by faeza 17.08.2022
        public bool? fld_AdaORP { get; set; }

        //added by faeza 26.02.2023
        public bool? fld_InclSecondPayslip { get; set; }

    }
}
