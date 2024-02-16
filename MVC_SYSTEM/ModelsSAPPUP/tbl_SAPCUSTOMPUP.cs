namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPCUSTOMPUP
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        //idzham tambah
        public int? fld_Year { get; set; }

        [StringLength(4)]
        public string fld_CompanyCode { get; set; }

        [StringLength(10)]
        public string fld_WilayahCode { get; set; }

        [StringLength(10)]
        public string fld_PktUtama { get; set; }

        [StringLength(10)]
        public string fld_Blok { get; set; }

        [StringLength(24)]
        public string fld_JnsTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LsPktUtama { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_DirianPokok { get; set; }

        [StringLength(24)]
        public string fld_WBSCode { get; set; }

        public int? fld_TahunTnm { get; set; }

        [StringLength(24)]
        public string fld_StatusTnmn { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? fld_LuasBerhasil { get; set; }

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

    public partial class CUSTOMITEM
    {
        //idzham tambah
        public int? ZGJAHR { get; set; }

        //[StringLength(4)]
        public int ZBUKRS { get; set; }

        [StringLength(10)]
        public string ZREGIO { get; set; }

        [StringLength(10)]
        public string ZDIVID { get; set; }

        [StringLength(10)]
        public string ZBLKID { get; set; }

        [StringLength(10)]
        public string ZCOSTC { get; set; }

        [StringLength(24)]
        public string ZCROPT { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ZHECTA { get; set; }

        public int? ZSPHEC { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? ZLSKBH { get; set; }

        [StringLength(24)]
        public string ZWBSCO { get; set; }

        public int? ZYEARP { get; set; }

        [StringLength(24)]
        public string ZCROPC { get; set; }

        //active status
        public string ZINDDLTD { get; set; }
    }
}
