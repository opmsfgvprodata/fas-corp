namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_SAPMasterAssetMapping
    {
        [Key]
        public int fld_ID { get; set; }

        public string fld_CompanyCode { get; set; }

        public string fld_AssetNo { get; set; }
    }
}
