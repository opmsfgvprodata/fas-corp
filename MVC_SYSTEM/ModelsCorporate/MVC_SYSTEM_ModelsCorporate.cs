namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_ModelsCorporate : DbContext
    {
        public MVC_SYSTEM_ModelsCorporate()
            : base(nameOrConnectionString: "MVC_SYSTEM_HQ_CONN")
        {
        }

        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tblPkjmastApp> tblPkjmastApp { get; set; }
        public virtual DbSet<vw_AuditTrail> vw_AuditTrail { get; set; }
        public virtual DbSet<vw_UserIDDetail> vw_UserIDDetail { get; set; }
        public virtual DbSet<tbl_ASCRawData> tbl_ASCRawData { get; set; }
        public virtual DbSet<tbl_ASCRawDataDetail> tbl_ASCRawDataDetail { get; set; }
        public virtual DbSet<tbl_AuditTrail> tbl_AuditTrail { get; set; }
        public virtual DbSet<tbl_Bank> tbl_Bank { get; set; }
        public virtual DbSet<tbl_EstateSelection> tbl_EstateSelection { get; set; }
        public virtual DbSet<tbl_KumpulanSyarikat> tbl_KumpulanSyarikat { get; set; }
        public virtual DbSet<tbl_ListASCFile> tbl_ListASCFile { get; set; }
        public virtual DbSet<tbl_LogDetail> tbl_LogDetail { get; set; }
        public virtual DbSet<tbl_Negara> tbl_Negara { get; set; }
        public virtual DbSet<tbl_OptionConfig> tbl_OptionConfig { get; set; }
        public virtual DbSet<tbl_Pembekal> tbl_Pembekal { get; set; }
        public virtual DbSet<tbl_PerluLadang> tbl_PerluLadang { get; set; }
        public virtual DbSet<tbl_PerluLadangHistory> tbl_PerluLadangHistory { get; set; }
        public virtual DbSet<tbl_Poskod> tbl_Poskod { get; set; }
        public virtual DbSet<tbl_QuotaPerluLadang> tbl_QuotaPerluLadang { get; set; }
        public virtual DbSet<tbl_QuotaPerluLadangHistory> tbl_QuotaPerluLadangHistory { get; set; }
        public virtual DbSet<tbl_ServicesList> tbl_ServicesList { get; set; }
        public virtual DbSet<tbl_SevicesProcess> tbl_SevicesProcess { get; set; }
        public virtual DbSet<tbl_SevicesProcessHistory> tbl_SevicesProcessHistory { get; set; }
        public virtual DbSet<tbl_SokPermhnWang> tbl_SokPermhnWang { get; set; }
        public virtual DbSet<tbl_SuperAdminSelection> tbl_SuperAdminSelection { get; set; }
        public virtual DbSet<tbl_Syarikat> tbl_Syarikat { get; set; }
        public virtual DbSet<tbl_UploadedCountDetail> tbl_UploadedCountDetail { get; set; }
        public virtual DbSet<tbl_Wilayah> tbl_Wilayah { get; set; }
        public virtual DbSet<tblAktiviti> tblAktivitis { get; set; }
        public virtual DbSet<tblASCApprovalFileDetail> tblASCApprovalFileDetails { get; set; }
        public virtual DbSet<tblASCApprovalRawData> tblASCApprovalRawDatas { get; set; }
        public virtual DbSet<tblClient> tblClients { get; set; }
        public virtual DbSet<tblConnection> tblConnections { get; set; }
        public virtual DbSet<tblDataEntryList> tblDataEntryLists { get; set; }
        public virtual DbSet<tblEmailList> tblEmailLists { get; set; }
        public virtual DbSet<tblEmailNotiStatu> tblEmailNotiStatus { get; set; }
        public virtual DbSet<tblHtmlReport> tblHtmlReport { get; set; }
        public virtual DbSet<tblMaintenanceList> tblMaintenanceLists { get; set; }
        public virtual DbSet<tblNgrSmbrSyrkt> tblNgrSmbrSyrkts { get; set; }
        public virtual DbSet<tblOptionConfigsWeb> tblOptionConfigsWebs { get; set; }
        public virtual DbSet<tblReportExport> tblReportExports { get; set; }
        public virtual DbSet<tblReportList> tblReportLists { get; set; }
        public virtual DbSet<tblRoleReport> tblRoleReports { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblSokPermhnWangHisAction> tblSokPermhnWangHisActions { get; set; }
        public virtual DbSet<tblStatusPkj> tblStatusPkjs { get; set; }
        public virtual DbSet<tblSubReportList> tblSubReportLists { get; set; }
        public virtual DbSet<tblSystemConfig> tblSystemConfigs { get; set; }
        public virtual DbSet<tblTaskRemainder> tblTaskRemainders { get; set; }
        public virtual DbSet<tblTKABatch> tblTKABatches { get; set; }
        public virtual DbSet<tblTKADetail> tblTKADetails { get; set; }
        public virtual DbSet<tblUserIDApp> tblUserIDApps { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tbl_Upah> tbl_Upah { get; set; }
        public virtual DbSet<tbl_UpahMenuai> tbl_UpahMenuai { get; set; }
        public virtual DbSet<tbl_CutiKategori> tbl_CutiKategori { get; set; }
        public virtual DbSet<tbl_CutiUmum> tbl_CutiUmum { get; set; }
        public virtual DbSet<tbl_CutiMaintenance> tbl_CutiMaintenance { get; set; }
        public virtual DbSet<tbl_JenisCaruman> tbl_JenisCaruman { get; set; }
        public virtual DbSet<tbl_Kwsp> tbl_Kwsp { get; set; }
        public virtual DbSet<tbl_Socso> tbl_Socso { get; set; }
        public virtual DbSet<vw_Socso> vw_Socso { get; set; }
        public virtual DbSet<tblMenuList> tblMenuLists { get; set; }
        public virtual DbSet<tbl_JenisInsentif> tbl_JenisInsentif { get; set; }
        public virtual DbSet<tbl_UpahAktiviti> tbl_UpahAktiviti { get; set; }
        public virtual DbSet<tbl_MingguNegeri> tbl_MingguNegeri { get; set; }
        public virtual DbSet<tbl_HariBekerja> tbl_HariBekerja { get; set; }
        public virtual DbSet<tbl_JenisAktiviti> tbl_JenisAktiviti { get; set; }
        public virtual DbSet<tbl_HargaSawitRange> tbl_HargaSawitRange { get; set; }
        public virtual DbSet<tbl_HargaSawitSemasa> tbl_HargaSawitSemasa { get; set; }
        public virtual DbSet<vw_ServicesProcess> vw_ServicesProcess { get; set; }
        public virtual DbSet<tbl_Lejar> tbl_Lejar { get; set; }
        public virtual DbSet<tbl_CarumanTambahan> tbl_CarumanTambahan { get; set; }
        public virtual DbSet<tbl_SubCarumanTambahan> tbl_SubCarumanTambahan { get; set; }
        public virtual DbSet<tbl_JadualCarumanTambahan> tbl_JadualCarumanTambahan { get; set; }
        public virtual DbSet<vw_MingguNegeri> vw_MingguNegeri { get; set; }
        public virtual DbSet<vw_HariBekerja> vw_HariBekerja { get; set; }
        public virtual DbSet<vw_NSWL> vw_NSWL { get; set; }
        public virtual DbSet<tbl_MapGL> tbl_MapGL { get; set; }
        public virtual DbSet<tbl_BlckKmskknDataKerja> tbl_BlckKmskknDataKerja { get; set; }
        public virtual DbSet<tbl_GLSAP> tbl_GLSAP { get; set; }
        public virtual DbSet<tbl_Sctran> tbl_Sctran { get; set; }
        public virtual DbSet<vw_DetailPekerja> vw_DetailPekerja { get; set; }
        public virtual DbSet<tbl_Pkjmast> tbl_Pkjmast { get; set; }
        public virtual DbSet<vw_GajiBulanan> vw_GajiBulanan { get; set; }
        public virtual DbSet<vw_NeragaSumberDetail> vw_NeragaSumberDetail { get; set; }
        public virtual DbSet<vw_TKADetail> vw_TKADetail { get; set; }
        public virtual DbSet<tbl_Version> tbl_Version { get; set; }
        public virtual DbSet<vw_PermohonanWang> vw_PermohonanWang { get; set; }
        public virtual DbSet<vw_PermitPassportDetail> vw_PermitPassportDetail { get; set; }
        public virtual DbSet<tbl_KategoriAktiviti> tbl_KategoriAktiviti { get; set; }
        public virtual DbSet<tbl_BlckKmskknDataKerjaHistory> tbl_BlckKmskknDataKerjaHistory { get; set; }
        public virtual DbSet<vw_PermohonanKewangan> vw_PermohonanKewangan { get; set; }
        public virtual DbSet<tbl_CostCentre> tbl_CostCentre { get; set; }
        public virtual DbSet<tbl_CCSAP> tbl_CCSAP { get; set; }
        public virtual DbSet<tblUserAuditTrail> tblUserAuditTrails { get; set; }
        public virtual DbSet<vw_HargaSemasa> vw_HargaSemasa { get; set; }
        public virtual DbSet<tbl_CutiUmumMaster> tbl_CutiUmumMaster { get; set; }
        public virtual DbSet<tbl_CutiUmumLdg> tbl_CutiUmumLdg { get; set; }
        public virtual DbSet<vw_CutiUmumLdg> vw_CutiUmumLdg { get; set; }
        public virtual DbSet<vw_CutiUmumLdgDetails> vw_CutiUmumLdgDetails { get; set; }
        public virtual DbSet<tbl_CutiUmumKelayakan> tbl_CutiUmumKelayakan { get; set; }
        public virtual DbSet<tbl_SAPCCPUP> tbl_SAPCCPUP { get; set; }
        public virtual DbSet<tbl_SAPGLPUP> tbl_SAPGLPUP { get; set; }
        public virtual DbSet<tbl_SAPCustomerPUP> tbl_SAPCustomerPUP { get; set; }
        public virtual DbSet<tbl_SAPPDPUP> tbl_SAPPDPUP { get; set; }
        public virtual DbSet<tbl_SAPVendorPUP> tbl_SAPVendorPUP { get; set; }
        public virtual DbSet<tbl_SAPCUSTOMPUP> tbl_SAPCUSTOMPUP { get; set; }
        public object tbl_MapGLViewModelCreate { get; internal set; }
        public virtual DbSet<tbl_AktivitiNNCCMap> tbl_AktivitiNNCCMap { get; set; }
        public virtual DbSet<tbl_AktvtEstate> tbl_AktvtEstate { get; set; }
        public virtual DbSet<tbl_KelayakanInsentifPkjLdg> tbl_KelayakanInsentifPkjLdg { get; set; }
        public virtual DbSet<tbl_KelayakanInsentifLdg> tbl_KelayakanInsentifLdg { get; set; }
        public virtual DbSet<vw_KelayakanInsentifLdg> vw_KelayakanInsentifLdg { get; set; }
        public virtual DbSet<tbl_JenisMingguLadang> tbl_JenisMingguLadang { get; set; }
        public virtual DbSet<vw_JenisMingguLadang> vw_JenisMingguLadang { get; set; }
        public virtual DbSet<tbl_HariBekerjaLadang> tbl_HariBekerjaLadang { get; set; }
        public virtual DbSet<vw_HariBekerjaLadang> vw_HariBekerjaLadang { get; set; }
        public virtual DbSet<vw_CutiUmumKelayakanDetails> vw_CutiUmumKelayakanDetails { get; set; }
        public virtual DbSet<vw_CutiUmumKelayakan> vw_CutiUmumKelayakan { get; set; }
        public virtual DbSet<tbl_PkjIncrmntApp> tbl_PkjIncrmntApp { get; set; }
        public virtual DbSet<vw_NSWL_2> vw_NSWL_2 { get; set; }
        public virtual DbSet<tbl_Division> tbl_Division { get; set; }
        public virtual DbSet<tbl_ModulesUrl> tbl_ModulesUrl { get; set; }
        public virtual DbSet<tbl_EstateDivisionSelection> tbl_EstateDivisionSelection { get; set; }

        //Added by Shazana on 20/10
        public virtual DbSet<tbl_CustomerVendorGLMap> tbl_CustomerVendorGLMap { get; set; }
        //Close Added by Shazana on 20/10

        //Added by Shazana on 15/12
        public virtual DbSet<sp_WagerateView> sp_WagerateView { get; set; }
        //Close Added by Shazana on 15/12

        //public virtual DbSet<tbl_KategoriAktiviti> tbl_KategoriAktivitiPUP { get; set; }

        public virtual DbSet<tbl_EAWorkerSiriNo> tbl_EAWorkerSiriNo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_SubCarumanTambahan>()
                .Property(e => e.fld_KadarMajikan)
                .HasPrecision(18, 4);

            modelBuilder.Entity<tbl_SubCarumanTambahan>()
                .Property(e => e.fld_KadarPekerja)
                .HasPrecision(18, 4);

            modelBuilder.Entity<tbl_SAPCUSTOMPUP>()
                .Property(e => e.fld_LsPktUtama)
                .HasPrecision(12, 2);

            modelBuilder.Entity<tbl_SAPCUSTOMPUP>()
                .Property(e => e.fld_DirianPokok)
                .HasPrecision(12, 2);

            modelBuilder.Entity<tbl_SAPCUSTOMPUP>()
                .Property(e => e.fld_LuasBerhasil)
                .HasPrecision(12, 2);

            modelBuilder.Entity<tblTaskRemainder>()
                .Property(e => e.fldPurpose)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_UserIDDetail>()
                .Property(e => e.fldNama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_UserIDDetail>()
                .Property(e => e.fldNoKP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_UserIDDetail>()
                .Property(e => e.fldKdLdg)
                .IsUnicode(false);

            modelBuilder.Entity<vw_UserIDDetail>()
                .Property(e => e.fldNamaLdg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_UserIDDetail>()
                .Property(e => e.fldJawatan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_UserIDDetail>()
                .Property(e => e.fldPassword)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<vw_UserIDDetail>()
                .Property(e => e.fldStatus)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldUserid)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNoKP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldKdLdg)
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNamaLdg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldJawatan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldPassword)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldStatus)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNoPkj)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNama1)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNoKP)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdJnsPkj)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdRkyt)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdLdg)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldJumPjm)
                .HasPrecision(7, 2);

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldSbbMsk)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldAlsnMsk)
                .IsFixedLength();

            modelBuilder.Entity<tbl_JenisCaruman>()
                .Property(e => e.fld_PeratusCarumanPekerja)
                .HasPrecision(18, 3);

            modelBuilder.Entity<tbl_JenisCaruman>()
                .Property(e => e.fld_PeratusCarumanMajikanBawah5000)
                .HasPrecision(18, 3);

            modelBuilder.Entity<tbl_JenisCaruman>()
                .Property(e => e.fld_PeratusCarumanMajikanAtas5000)
                .HasPrecision(18, 3);

            modelBuilder.Entity<tbl_MapGL>()
                .Property(e => e.fld_KodAktvt)
                .IsFixedLength();

            modelBuilder.Entity<tbl_MapGL>()
                .Property(e => e.fld_KodGL)
                .IsFixedLength();

            modelBuilder.Entity<tbl_MapGL>()
                .Property(e => e.fld_Paysheet)
                .IsFixedLength();

            modelBuilder.Entity<tbl_Lejar>()
                .Property(e => e.fld_KodCaj)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Poskod>()
                .Property(e => e.fld_Postcode)
                .IsFixedLength();

            modelBuilder.Entity<tbl_Poskod>()
                .Property(e => e.fld_DistrictArea)
                .IsFixedLength();

            modelBuilder.Entity<tbl_Poskod>()
                .Property(e => e.fld_State)
                .IsFixedLength();

            modelBuilder.Entity<tbl_Poskod>()
                .Property(e => e.fld_Region)
                .IsFixedLength();

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahPermohonan)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahPDP)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahTT)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahCIT)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tbl_SokPermhnWang>()
                .Property(e => e.fld_JumlahManual)
                .HasPrecision(13, 2);

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNoPkj)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNama1)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldNoKP)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdJnsPkj)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdRkyt)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldKdLdg)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldJumPjm)
                .HasPrecision(7, 2);

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldSbbMsk)
                .IsFixedLength();

            modelBuilder.Entity<tblPkjmastApp>()
                .Property(e => e.fldAlsnMsk)
                .IsFixedLength();

            modelBuilder.Entity<tblStatusPkj>()
                .Property(e => e.fldNoPkjLama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblStatusPkj>()
                .Property(e => e.fldNoKP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblStatusPkj>()
                .Property(e => e.fldNoPkjBaru)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblTaskRemainder>()
                .Property(e => e.fldPurpose)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldUserid)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNoKP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldKdLdg)
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldNamaLdg)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldJawatan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldPassword)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblUserIDApp>()
                .Property(e => e.fldStatus)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblStatusPkj>()
                .Property(e => e.fldNoPkjLama)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblStatusPkj>()
                .Property(e => e.fldNoKP)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tblStatusPkj>()
                .Property(e => e.fldNoPkjBaru)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<tbl_Upah>()
               .Property(e => e.fld_Harga)
               .HasPrecision(18, 3);

            modelBuilder.Entity<tbl_UpahAktiviti>()
                .Property(e => e.fld_KdhByr)
                .IsFixedLength()
                .IsUnicode(false);

            //added by faeza 03.06.2022
            modelBuilder.Entity<tbl_UpahAktiviti>()
                .Property(e => e.fld_Harga)
                .HasPrecision(18,3);
        }
    }
}
