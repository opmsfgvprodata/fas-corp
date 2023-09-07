namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_MasterPkj
    {
        [Key]
        [StringLength(10)]
        public string fld_ID { get; set; }

        [StringLength(10)]
        public string fld_Nopkj { get; set; }

        [StringLength(40)]
        public string fld_Nama { get; set; }

        [StringLength(12)]
        public string fld_Nokp { get; set; }

        [StringLength(10)]
        public string fld_IDpkj { get; set; }

        [StringLength(1)]
        public string fld_Kdaktf { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }
    }
}
