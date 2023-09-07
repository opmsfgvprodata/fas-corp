//Added by Shazana on 15/12
namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class sp_WagerateView
    {
        [Key]
        public int fld_idUpahAktiviti { get; set; }
        public int RowtempUpahAktvt { get; set; }
        [StringLength(4)]
        public string fld_KodAktvt { get; set; }
        [StringLength(150)]
        public string fld_Desc { get; set; }
        public Decimal? fld_Harga { get; set; }
        [StringLength(4)]
        public string fld_KategoriAktvt { get; set; }
        public int fld_WilayahID { get; set; }
        public int fld_NegaraID { get; set; }
        public int fld_SyarikatID { get; set; }
        public int RowtempMapGL { get; set; }
        public int fld_idMapGL { get; set; }
        [StringLength(50)]
        public string fld_Paysheet { get; set; }
    }
}
