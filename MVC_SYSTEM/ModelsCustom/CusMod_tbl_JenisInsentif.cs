using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsCustom;

namespace MVC_SYSTEM.ModelsCustom
{
    public class CusMod_tbl_JenisInsentif
    {

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

        [StringLength(10)]
        public string fld_KodGL { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(10)]
        public string fld_KodGLTKA { get; set; }

        [StringLength(10)]
        public string fld_KodGLTKT { get; set; }

        //added by faeza 17.08.2022
        public bool? fld_AdaORP { get; set; }

        //added by faeza 26.02.2023
        public bool? fld_InclSecondPayslip { get; set; }

    }
}

