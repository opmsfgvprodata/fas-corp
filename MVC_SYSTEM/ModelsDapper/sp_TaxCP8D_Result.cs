using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsDapper
{
    public class TaxCP8D_Result
    {
        public int ID { get; set; }
        public string NoPkerja { get; set; }
        public string NamaPkerja { get; set; }
        public string TINNo { get; set; }
        public string IDNo { get; set; }
        public string KategoryPekerja { get; set; }
        public short StatusPekerja { get; set; }
        public DateTime? TarikhAkhirBekerja { get; set; }
        public short MajikanTanggungCukai { get; set; }
        public int BilanganAnak { get; set; }
        public int JumlahPelepasanAnak { get; set; }
        public int JumlahSaraanKasar { get; set; }
        public int ManfaatBarangan { get; set; }
        public int NilaiKediaman { get; set; }
        public int ESOS { get; set; }
        public int ElaunDikecualikan { get; set; }
        public int JumlahTuntutanPelepasan { get; set; }
        public decimal JumlahTututanZakat { get; set; }
        public int KWSP { get; set; }
        public decimal ZakatPotonganGaji { get; set; }
        public decimal PCB { get; set; }
        public decimal CP38 { get; set; }
        public int InsuransPotonganGaji { get; set; }
        public int PERKESO { get; set; }
        public string EstateName { get; set; }
    }

    public class WorkerInfo
    {
        public int? fld_LadangID { get; set; }
        public int? fld_DivisionID { get; set; }
        public string fld_Nama { get; set; }
        public string fld_NoPkjPermanent { get; set; }
        public string fld_Nopkj { get; set; }
        public string fld_Nokp { get; set; }
    }

    public class WorkerTaxCP8D
    {
        public Guid fld_WorkerTaxID { get; set; }
        public string fld_NoPkjPermanent { get; set; }
        public string fld_Nama { get; set; }
        public decimal fld_KWSPPkj { get; set; }
        public decimal fld_SocsoPkj { get; set; }
        public decimal fld_PCB { get; set; }
        public decimal fld_Zakat { get; set; }
        public decimal fld_CP38 { get; set; }
        public decimal fld_PelepasanAnak { get; set; }
        public decimal fld_SaraanKasar { get; set; }
        public int fld_Month { get; set; }
        public int fld_Year { get; set; }
        public int? fld_LadangID { get; set; }
        public int? fld_DivisionID { get; set; }
        public string fld_TaxNo { get; set; }
        public string fld_Nokp { get; set; }
        public string fld_Kdrkyt { get; set; }
        public string fld_TaxMaritalStatus { get; set; }
        public int fld_ChildAbove18CertFull { get; set; }
        public int fld_ChildAbove18CertHalf { get; set; }
        public int fld_ChildAbove18HigherFull { get; set; }
        public int fld_ChildAbove18HigherHalf { get; set; }
        public int fld_ChildBelow18Full { get; set; }
        public int fld_ChildBelow18Half { get; set; }
        public int fld_DisabledChildFull { get; set; }
        public int fld_DisabledChildHalf { get; set; }
        public int fld_DisabledChildStudyFull { get; set; }
        public int fld_DisabledChildStudyHalf { get; set; }
        public DateTime? fld_ContractExpiryDate { get; set; }
        public DateTime? fld_Trlhr { get; set; }
    }

    public class OtherContribution
    {
        public string fld_NopkjPermanent { get; set; }
        public decimal fld_CarumanPekerja { get; set; }
        public string fld_KodCaruman { get; set; }
        public int fld_Month { get; set; }
        public int fld_Year { get; set; }
    }

    public class SpecialIncentive
    {
        public string fld_Nopkj { get; set; }
        public string fld_KodInsentif { get; set; }
        public decimal fld_NilaiInsentif { get; set; }
        public int fld_Month { get; set; }
        public int fld_Year { get; set; }
        public decimal fld_PCBCarumanPekerja { get; set; }
        public decimal fld_KWSPPkj { get; set; }
    }
}