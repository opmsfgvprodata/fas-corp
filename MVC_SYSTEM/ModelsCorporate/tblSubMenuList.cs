namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("tblSubMenuList")]
    public partial class tblSubMenuList
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(50)]
        public string fld_Val { get; set; }

        [StringLength(50)]
        public string fld_Desc { get; set; }

        [StringLength(50)]
        public string fld_Flag { get; set; }

        public bool? fldDeleted { get; set; }
    }
}
