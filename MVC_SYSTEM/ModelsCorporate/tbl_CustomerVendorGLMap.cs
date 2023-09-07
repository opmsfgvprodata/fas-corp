namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_CustomerVendorGLMap
    {
        //Added by Shazana on 20/10
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid fld_ID { get; set; }

        [StringLength(10)]
        public string fld_KodAktiviti { get; set; }

        [StringLength(10)]
        public string fld_Flag { get; set; }

        [StringLength(12)]
        public string fld_TypeCode { get; set; }

        [StringLength(12)]
        public string fld_SAPCode { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WIlayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
