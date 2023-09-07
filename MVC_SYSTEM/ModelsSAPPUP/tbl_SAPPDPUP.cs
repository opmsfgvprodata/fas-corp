namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPPDPUP
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(50)]
        public string fld_ProjectDefinition { get; set; }

        [StringLength(50)]
        public string fld_ProjectDesc { get; set; }

        [StringLength(50)]
        public string fld_WBSElement { get; set; }

        [StringLength(50)]
        public string fld_WBSDesc { get; set; }

        [StringLength(50)]
        public string fld_WBSCode { get; set; }

        [StringLength(50)]
        public string fld_WorkActivity { get; set; }

        [StringLength(50)]
        public string fld_ActivityType { get; set; }

        [StringLength(50)]
        public string fld_NetworkNo { get; set; }

        [StringLength(50)]
        public string fld_ActivityCode { get; set; }

        [StringLength(200)]
        public string fld_ActivityDesc { get; set; }

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

        [StringLength(2)]
        public string fld_KodKategori { get; set; }

        [StringLength(20)]
        public string fld_CostCenter { get; set; }
    }

    public partial class PDITEM
    {
        [StringLength(50)]
        public string PROJ_PSPID { get; set; }

        [StringLength(50)]
        public string PROJ_POST1 { get; set; }

        [StringLength(50)]
        public string PRPS_POSID { get; set; }

        [StringLength(50)]
        public string PRPS_POST1 { get; set; }

        [StringLength(50)]
        public string CAUFVD_AUFNR { get; set; }

        [StringLength(50)]
        public string AFVGD_VORNR { get; set; }

        [StringLength(200)]
        public string AFVGM_LTXA1 { get; set; }

        [StringLength(10)]
        public string PRPS_FKSTL { get; set; }

        public string STATUS { get; set; }
    }
}
