namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPMasterAsset
    {
        [Key]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public Guid fld_id { get; set; }

        [StringLength(4)]
        public string fld_CompanyCode { get; set; }

        [StringLength(12)]
        public string fld_AssetNo { get; set; }

        [StringLength(4)]
        public string fld_AssetSubNo { get; set; }

        [StringLength(8)]
        public string fld_AssetClass { get; set; }

        [StringLength(50)]
        public string fld_AssetModel { get; set; }

        [StringLength(25)]
        public string fld_InventoryNo { get; set; }

        [StringLength(13)]
        public string fld_AssetQty { get; set; }

        [StringLength(3)]
        public string fld_AssetUOM { get; set; }

        [StringLength(10)]
        public string fld_CostCenter { get; set; }

        [StringLength(4)]
        public string fld_BusinessArea { get; set; }

        [StringLength(15)]
        public string fld_AssetPlateNo { get; set; }

        [StringLength(8)]
        public string fld_WBSNo { get; set; }

        [StringLength(8)]
        public string fld_CapitalizedOn { get; set; }

        [StringLength(8)]
        public string fld_Acquitision { get; set; }

        [StringLength(12)]
        public string fld_CreatedBy { get; set; }

        [StringLength(8)]
        public string fld_CreatedOn { get; set; }


    }

    public partial class MASTERASSETITEM
    {
        //[StringLength(4)]
        public string ANLA_BUKRS { get; set; }
        public string ANLA_ANLN1 { get; set; }
        public string ANLA_ANLN2 { get; set; }
        public string ANLA_ANLKL { get; set; }
        public string ANLA_TXT50 { get; set; }
        public string ANLA_INVNR { get; set; }
        public string ANLA_MENGE { get; set; }
        public string ANLA_MEINS { get; set; }
        public string ANLA_KOSTL { get; set; }
        public string ANLA_GSBER { get; set; }
        public string ANLA_KFZKZ { get; set; }
        public string ANLA_POSNR { get; set; }
        public string ANLA_AKTIV { get; set; }
        public string ANLA_ZUGDT { get; set; }
        public string ANLA_ERNAM { get; set; }
        public string ANLA_ERDAT { get; set; }
    }
}
