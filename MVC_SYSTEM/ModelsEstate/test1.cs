namespace MVC_SYSTEM.ModelsEstate
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class test1 : DbContext
    {
        public test1()
            : base("name=test1")
        {
        }

        public virtual DbSet<tbl_CutiPeruntukan> tbl_CutiPeruntukan { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
