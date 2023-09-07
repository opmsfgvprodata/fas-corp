namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_JadualCarumanTambahan
    {
        [Key]
        public int fld_JadualCarumanTambahanID { get; set; }

        [StringLength(10)]
        public string fld_KodSubCaruman { get; set; }

        public decimal? fld_GajiLower { get; set; }

        public decimal? fld_GajiUpper { get; set; }

        public decimal? fld_CarumanPekerja { get; set; }

        public decimal? fld_CarumanMajikan { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_NegaraID { get; set; }

        public bool fld_Deleted { get; set; }
    }
}
