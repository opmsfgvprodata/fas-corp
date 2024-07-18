using Newtonsoft.Json;

namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPCCPUP
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_ID { get; set; }

        [StringLength(4)]
        public string fld_CompanyCode { get; set; }

        [StringLength(10)]
        public string fld_CostCenter { get; set; }

        [StringLength(50)]
        public string fld_CostCenterDesc { get; set; }

        public short? fld_CCIOStatus { get; set; }

        //[StringLength(10)]
        //public string fld_JenisAktiviti { get; set; }

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

    public partial class CCITEM
    {
        //[StringLength(10)]
        public int CSKS_BUKRS { get; set; }

        //[JsonProperty("CSKS_KOSTL")]
        [StringLength(10)]
        public string CSKS_KOSTL { get; set; }

        //[JsonProperty("CSKT_LTEXT")]
        [StringLength(50)]
        public string CSKT_LTEXT { get; set; }

        [StringLength(8)]
        public string CSKS_DATAB { get; set; }

        [StringLength(8)]
        public string CSKS_DATBI { get; set; }

        [StringLength(4)]
        public string CSKS_GSBER { get; set; }

        public string CSKS_BKZKP { get; set; }
    }

    public partial class IOITEM
    {
        //[JsonProperty("CSKS_KOSTL")]
        [StringLength(10)]
        public string AUFK_BUKRS { get; set; }

        //[JsonProperty("CSKT_LTEXT")]
        [StringLength(50)]
        public string AUFK_AUFNR { get; set; }

        public string AUFK_KTEXT { get; set; }
    }
}
