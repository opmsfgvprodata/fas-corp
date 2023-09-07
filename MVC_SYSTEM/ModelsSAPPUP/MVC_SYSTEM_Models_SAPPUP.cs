namespace MVC_SYSTEM.ModelsSAPPUP
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class MVC_SYSTEM_Models_SAPPUP : DbContext
    {
        public MVC_SYSTEM_Models_SAPPUP()
            : base("name=MVC_SYSTEM_HQ_CONN")
        {
        }

        public virtual DbSet<tbl_SAPCCPUP> tbl_SAPCCPUP { get; set; }
        public virtual DbSet<tbl_SAPCustomerPUP> tbl_SAPCustomerPUP { get; set; }
        public virtual DbSet<tbl_SAPGLPUP> tbl_SAPGLPUP { get; set; }
        public virtual DbSet<tbl_SAPVendorPUP> tbl_SAPVendorPUP { get; set; }
        public virtual DbSet<tbl_Syarikat> tbl_Syarikat { get; set; }
        public virtual DbSet<tbl_Negara> tbl_Negara { get; set; }
        public virtual DbSet<tbl_Ladang> tbl_Ladang { get; set; }
        public virtual DbSet<tbl_SAPPDPUP> tbl_SAPPDPUP { get; set; }
        public virtual DbSet<tbl_SAPCUSTOMPUP> tbl_SAPCUSTOMPUP { get; set; }
        public virtual DbSet<tbl_SAPLogPUP> tbl_SAPLogPUP { get; set; }
        public virtual DbSet<tbl_SAPOPMSCCLdgMapping> tbl_SAPOPMSCCLdgMapping { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<tbl_SAPCUSTOMPUP>()
                .Property(e => e.fld_LsPktUtama)
                .HasPrecision(12, 2);

            modelBuilder.Entity<tbl_SAPCUSTOMPUP>()
                .Property(e => e.fld_DirianPokok)
                .HasPrecision(12, 2);

            modelBuilder.Entity<tbl_SAPCUSTOMPUP>()
                .Property(e => e.fld_LuasBerhasil)
                .HasPrecision(12, 2);
        }
    }
}
