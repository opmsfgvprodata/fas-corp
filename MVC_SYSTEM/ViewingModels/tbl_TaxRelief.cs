namespace MVC_SYSTEM.ViewingModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_TaxRelief
    {
        [Key]
        public int fld_TaxReliefID { get; set; }

        [Required]
        [StringLength(10)]
        public string fld_TaxReliefCode { get; set; }

        [Required]
        [StringLength(200)]
        public string fld_TaxReliefItem { get; set; }

        public decimal fld_TaxReliefLimit { get; set; }

        [StringLength(10)]
        public string fld_VariableCode { get; set; }

        [StringLength(50)]
        public string fld_Remark { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public bool fld_Deleted { get; set; }

        //public DateTime fld_CreatedDT { get; set; }

        //public int fld_CreatedBy { get; set; }
        //public DateTime fld_ModifiedDT { get; set; }
        //public int fld_ModifiedBy { get; set; }
    }
}
