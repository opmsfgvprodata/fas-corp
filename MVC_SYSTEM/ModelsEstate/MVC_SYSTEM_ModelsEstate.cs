namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_ModelsEstate : DbContext
    {
        public static string host1 = "";
        public static string catalog1 = "";
        public static string user1 = "";
        public static string pass1 = "";

        public MVC_SYSTEM_ModelsEstate()
            : base(nameOrConnectionString: "BYOWN")
        {
            base.Database.Connection.ConnectionString = "data source=" + host1 + ";initial catalog=" + catalog1 + ";user id=" + user1 + ";password=" + pass1 + ";MultipleActiveResultSets=True;App=EntityFramework";
        }

        public static MVC_SYSTEM_ModelsEstate ConnectToSqlServer(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_ModelsEstate();
        }

        public MVC_SYSTEM_ModelsEstate ConnectToSqlServerMobile(string host, string catalog, string user, string pass)
        {
            host1 = host;
            catalog1 = catalog;
            user1 = user;
            pass1 = pass;

            return new MVC_SYSTEM_ModelsEstate();
        }

        public virtual DbSet<tbl_AktvtEstate> tbl_AktvtEstate { get; set; }
        public virtual DbSet<tbl_Blok> tbl_Blok { get; set; }
        public virtual DbSet<tbl_ByrCarumanTambahan> tbl_ByrCarumanTambahan { get; set; }
        public virtual DbSet<tbl_CutiDiambil> tbl_CutiDiambil { get; set; }
        public virtual DbSet<tbl_CutiPeruntukan> tbl_CutiPeruntukan { get; set; }
        public virtual DbSet<tbl_GajiBulanan> tbl_GajiBulanan { get; set; }
        public virtual DbSet<tbl_GajiMinima> tbl_GajiMinima { get; set; }
        public virtual DbSet<tbl_HasilSawitBlok> tbl_HasilSawitBlok { get; set; }
        public virtual DbSet<tbl_HasilSawitPkt> tbl_HasilSawitPkt { get; set; }
        public virtual DbSet<tbl_HasilSawitSubPkt> tbl_HasilSawitSubPkt { get; set; }
        public virtual DbSet<tbl_Insentif> tbl_Insentif { get; set; }
        public virtual DbSet<tbl_IO> tbl_IO { get; set; }
        public virtual DbSet<tbl_KawTidakBerhasil> tbl_KawTidakBerhasil { get; set; }
        public virtual DbSet<tbl_Kepuasan> tbl_Kepuasan { get; set; }
        public virtual DbSet<tbl_Kerja> tbl_Kerja { get; set; }
        public virtual DbSet<tbl_KerjaBonus> tbl_KerjaBonus { get; set; }
        public virtual DbSet<tbl_KerjaHariTerabai> tbl_KerjaHariTerabai { get; set; }
        public virtual DbSet<tbl_Kerjahdr> tbl_Kerjahdr { get; set; }
        public virtual DbSet<tbl_KerjahdrCuti> tbl_KerjahdrCuti { get; set; }
        public virtual DbSet<tbl_KerjahdrCutiTahunan> tbl_KerjahdrCutiTahunan { get; set; }
        public virtual DbSet<tbl_KerjaOT> tbl_KerjaOT { get; set; }
        public virtual DbSet<tbl_KerjaSAPData> tbl_KerjaSAPData { get; set; }
        public virtual DbSet<tbl_KumpulanKerja> tbl_KumpulanKerja { get; set; }
        public virtual DbSet<tbl_MklmtKeluargaPkj> tbl_MklmtKeluargaPkj { get; set; }
        public virtual DbSet<tbl_PdfGen> tbl_PdfGen { get; set; }
        public virtual DbSet<tbl_PkjCarumanTambahan> tbl_PkjCarumanTambahan { get; set; }
        public virtual DbSet<tbl_PkjIncrmntSalary> tbl_PkjIncrmntSalary { get; set; }
        public virtual DbSet<tbl_PkjIncrmntSalaryHistory> tbl_PkjIncrmntSalaryHistory { get; set; }
        public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        public virtual DbSet<tbl_PktUtama> tbl_PktUtama { get; set; }
        public virtual DbSet<tbl_PktUtamaOthr> tbl_PktUtamaOthr { get; set; }
        public virtual DbSet<tbl_Produktiviti> tbl_Produktiviti { get; set; }
        public virtual DbSet<tbl_SAPPostDataDetails> tbl_SAPPostDataDetails { get; set; }
        public virtual DbSet<tbl_SAPPostRef> tbl_SAPPostRef { get; set; }
        public virtual DbSet<tbl_SAPPostReturn> tbl_SAPPostReturn { get; set; }
        public virtual DbSet<tbl_Sctran> tbl_Sctran { get; set; }
        public virtual DbSet<tbl_Skb> tbl_Skb { get; set; }
        public virtual DbSet<tbl_SubPkt> tbl_SubPkt { get; set; }
        public virtual DbSet<tbl_SupportedDoc> tbl_SupportedDoc { get; set; }
        public virtual DbSet<tbl_TamatPermitPassport> tbl_TamatPermitPassport { get; set; }
        public virtual DbSet<tbl_TutupUrusNiaga> tbl_TutupUrusNiaga { get; set; }
        public virtual DbSet<tblStatusPkj> tblStatusPkjs { get; set; }
        public virtual DbSet<tbl_LogDetail> tbl_LogDetail { get; set; }
        public virtual DbSet<tbl_Photo> tbl_Photo { get; set; }
        public virtual DbSet<tbl_ServicesList> tbl_ServicesList { get; set; }
        public virtual DbSet<tbl_SevicesProcess> tbl_SevicesProcess { get; set; }
        public virtual DbSet<tbl_SevicesProcessHistory> tbl_SevicesProcessHistory { get; set; }
        public virtual DbSet<tblHtmlReport> tblHtmlReports { get; set; }
        public virtual DbSet<vw_KumpulanKerja> vw_KumpulanKerja { get; set; }
        public virtual DbSet<vw_PermitPassportDetail> vw_PermitPassportDetail { get; set; }
        public virtual DbSet<vw_MaklumatInsentif> vw_MaklumatInsentif { get; set; }
        public virtual DbSet<tbl_PktPinjam> tbl_PktPinjam { get; set; }
        public virtual DbSet<tbl_TaxWorkerInfo> tbl_TaxWorkerInfo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    }
}
