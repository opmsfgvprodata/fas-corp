using MVC_SYSTEM.App_LocalResources;
namespace MVC_SYSTEM.ModelsCustom
{  using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

   
    public class CusMod_tbl_UpahAktiviti
    {
      
        [Key]
        public int fld_ID { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(150)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        //modified by faeza 03.06.2022 - change 9999999.99 to 9999999.999
        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(2)]
        public string fld_KategoriAktvt { get; set; }
        public string fld_GLCode { get; set; }
        
        //Added by Shazana on 5/5/2021
        public string fld_GLCodePT { get; set; }

        public string RadioGroup { get; set; }
        public string fld_KodJnsAktvt { get; set; }

        //Added by Shazana on 15/12
        public int? rn { get; set; }
        public int? fld_idUpahAktiviti { get; set; }
        public int? RowtempUpahAktvt { get; set; }
        public int? RowtempMapGL { get; set; }
        public int? fld_idMapGL { get; set; }
        [StringLength(50)]
        public string fld_Paysheet { get; set; }
    }

    public partial class tbl_UpahAktivitiViewModel
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        //modified by faeza 03.06.2022 - change 9999999.99 to 9999999.999
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,2})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        //Added by Shazana on 29/10
        [StringLength(2)]
        public string fld_KategoriAktvt { get; set; }
        public string fld_GLCode { get; set; }

        //Close Added by Shazana on 29/10

        //Added by Shazana on 15/12
        public int? fld_idMapGL { get; set; }
        public int? fld_idUpahAktiviti { get; set; }
        //Close Added by Shazana on 15/12
        public string fld_GLCodePT { get; set; } //Added by Shazana on 5/5/2021

    }

    //Commented by Shazana on 29/10
    //[Table("tbl_UpahAktiviti")]
    public partial class tbl_UpahAktivitiViewModelCreate
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        //modified by faeza 03.06.2022 - change 9999999.99 to 9999999.999
        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        [StringLength(2)]
        public string fld_KategoriAktvt { get; set; } //Added by Shazana on 27/8

    }

    //[Table("tbl_UpahAktiviti")]
    public partial class tbl_UpahAktivitiViewModelGMN
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        //modified by faeza 03.06.2022 - change 9999999.99 to 9999999.999
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

    }

    //[Table("tbl_UpahAktiviti")]
    public partial class tbl_UpahAktivitiViewModelGMNEdit
    {
        [Key]
        public int fld_ID { get; set; }

        [StringLength(4)]
        public string fld_KodAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(100)]
        public string fld_Desc { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        [StringLength(10)]
        public string fld_Unit { get; set; }

        //modified by faeza 03.06.2022 - change 9999999.99 to 9999999.999
        [Range(0, 9999999.999, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_Harga { get; set; }

        [StringLength(2)]
        public string fld_KodJenisAktvt { get; set; }

        [Required(ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgModelValidation")]
        public short? fld_DisabledFlag { get; set; }

        [StringLength(1)]
        public string fld_KdhByr { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }

        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgMaxCurrencyModelValidation")]
        [RegularExpression("^\\d+(?:\\.\\d{1,3})?$", ErrorMessageResourceType = typeof(GlobalResCorp), ErrorMessageResourceName = "msgNumberModelValidation")]
        public decimal? fld_MaxProduktiviti { get; set; }

        [StringLength(1)]
        public string fld_Kategori { get; set; }

        [StringLength(2)]
        public string fld_KategoriAktvt { get; set; }

        //Added by Shazana on 29/10
        [StringLength(2)]
        public string fld_GLCode { get; set; }

    }
}