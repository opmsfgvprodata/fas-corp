namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_TaxEForm
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(100)]
        public string fld_NameofEmployer { get; set; }

        [StringLength(50)]
        public string fld_EmployerTIN { get; set; }

        [StringLength(50)]
        public string fld_CatofEmployer { get; set; }

        [StringLength(50)]
        public string fld_StatusofEmployer { get; set; }  
        
        [StringLength(50)]
        public string fld_TINCode { get; set; }

        [StringLength(50)]
        public string fld_TIN { get; set; }

        [StringLength(50)]
        public string fld_IdentificationNo { get; set; }

        [StringLength(50)]
        public string fld_PassportNo { get; set; } 
        
        [StringLength(50)]
        public string fld_RegNoCompaniesSSM { get; set; }
        
        [StringLength(70)]
        public string fld_Address1 { get; set; }  
        
        [StringLength(70)]
        public string fld_Address2 { get; set; } 
        
        [StringLength(70)]
        public string fld_Address3 { get; set; }

        [StringLength(50)]
        public string fld_Postcode { get; set; }

        [StringLength(50)]
        public string fld_City { get; set; }

        [StringLength(50)]
        public string fld_State { get; set; }

        [StringLength(50)]
        public string fld_Country { get; set; }
        
        [StringLength(50)]
        public string fld_PhoneNo { get; set; }
        
        [StringLength(50)]
        public string fld_HandphoneNo { get; set; }
        
        [StringLength(50)]
        public string fld_Email { get; set; }
        
        [StringLength(5)]
        public string fld_FurnishofCP8D { get; set; }

        public int? fld_A1 { get; set; }
        public int? fld_A2 { get; set; }
        public int? fld_A3 { get; set; }
        public int? fld_A4 { get; set; }
        public int? fld_A5 { get; set; }

        [StringLength(5)]
        public string fld_A6 { get; set; }

        [StringLength(100)]
        public string fld_DeclareName { get; set; }
        
        [StringLength(50)]
        public string fld_DeclareICNo { get; set; }
        
        public DateTime? fld_DeclareDate { get; set; }

        [StringLength(50)]
        public string fld_DeclareDesignation { get; set; }

        public int? fld_Year { get; set; }

        [StringLength(10)]
        public string fld_SyarikatID { get; set; }
        public int? fld_NegaraID { get; set; }

        [StringLength(50)]
        public string fld_CreatedBy { get; set; }

        public DateTime? fld_CreatedDate { get; set; }

        [StringLength(50)]
        public string fld_ModifiedBy { get; set; }

        public DateTime? fld_ModifiedDate { get; set; }

    }
}
