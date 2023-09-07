namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_IOSAP
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(15)]
        public string fld_IOcode { get; set; }

        [StringLength(5)]
        public string fld_PktCode { get; set; }

        [StringLength(5)]
        public string fld_SubPktCode { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasPkt { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasKawBerhasil { get; set; }

        [StringLength(1)]
        public string fld_LdgIndicator { get; set; }

        [StringLength(5)]
        public string fld_LdgKod { get; set; }

        public int? fld_StatusUsed { get; set; }

        [StringLength(3)]
        public string fld_JnsLot { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        public DateTime? fld_DTCreated { get; set; }

        public DateTime? fld_DTModified { get; set; }
    }
}
