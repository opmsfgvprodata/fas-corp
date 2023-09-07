namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class test2 : DbContext
    {
        public test2()
            : base("name=test2")
        {
        }

        public virtual DbSet<vw_KumpulanKerja> vw_KumpulanKerja { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
