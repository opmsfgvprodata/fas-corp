namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPVendorPUP
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(4)]
        public string fld_CompanyCode { get; set; }

        [StringLength(10)]
        public string fld_VendorCode { get; set; }

        [StringLength(50)]
        public string fld_VendorName { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_IsSelected { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(50)]
        public string fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDT { get; set; }

        public DateTime? fld_ModifiedDT { get; set; }

        [StringLength(50)]
        public string fld_ModifiedBy { get; set; }
    }

    public partial class VENDORITEM
    {
        //[StringLength(4)]
        public int LFB1_BUKRS { get; set; }

        [StringLength(10)]
        public string LFB1_LIFNR { get; set; }

        [StringLength(50)]
        public string LFA1_NAME1 { get; set; }

        public string LFB1_LOEVM { get; set; }
    }
}
