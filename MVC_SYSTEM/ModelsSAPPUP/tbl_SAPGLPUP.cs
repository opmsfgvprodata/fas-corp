namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPGLPUP
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(4)]
        public string fld_CompanyCode { get; set; }

        [StringLength(10)]
        public string fld_GLCode { get; set; }

        [StringLength(50)]
        public string fld_GLDesc { get; set; }

        public string fld_AccType { get; set; }
        public string fld_BSAcc { get; set; }
        public string fld_BlockPost { get; set; }
        public string fld_Status { get; set; }

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

    public partial class GLITEM
    {
        //[StringLength(4)]
        public int SKB1_BUKRS { get; set; }

        [StringLength(10)]
        public string SKB1_SAKNR { get; set; }

        [StringLength(50)]
        public string SKAT_TXT50 { get; set; }

        public string SKB1_XLOEB { get; set; }
        public string SKA1_GVTYP { get; set; }
        public string SKA1_XBILK { get; set; }
        public string SKAB1_XSPEB { get; set; }
    }

    public partial class GLITEMS
    {
        public GLITEM GLITEM { get; set; }
        //[StringLength(4)]
        //public string SKB1_BUKRS { get; set; }

        //[StringLength(10)]
        //public string SKB1_SAKNR { get; set; }

        //[StringLength(50)]
        //public string SKAT_TXT50 { get; set; }
    }
}
