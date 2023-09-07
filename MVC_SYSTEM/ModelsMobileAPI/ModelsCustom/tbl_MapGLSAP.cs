namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_MapGLSAP
    {
        [Key]
        public int fld_MapGLID { get; set; }
        
        public string fld_AktvtKod { get; set; }
        
        public string fld_KodGL { get; set; }
    }
}
