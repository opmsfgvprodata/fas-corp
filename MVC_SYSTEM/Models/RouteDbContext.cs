using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Models
{
    public class RouteDbContext<TContext> : DbContext where TContext : DbContext
    {
        static RouteDbContext()
        {
            Database.SetInitializer<TContext>(null);
        }

        protected RouteDbContext() : base(nameOrConnectionString: "MVC_SYSTEM_CONN")
        {
            using(MVC_SYSTEM_Models context = new MVC_SYSTEM_Models())
            {
                string dtsrc, incat, usid, pssd;

                dtsrc = "PC5340005684";
                incat = "CheckRollCorp";
                usid = "sa12";
                pssd = "abcd1234";

                base.Database.Connection.ConnectionString = "data source="+ dtsrc + ";initial catalog=" + incat + ";user id=" + usid + ";password=" + pssd + ";MultipleActiveResultSets=True;App=EntityFramework";
            }
        }
    }
}