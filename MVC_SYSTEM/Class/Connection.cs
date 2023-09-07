using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsEstate;

namespace MVC_SYSTEM.Class
{
    public class Connection
    {
        public void GetConnection(out string host, out string catalog, out string user, out string pass, int? wlyhID, int? syrktID, int? ngrID)
        {
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            var getconnection = dbhq.tblConnections.Where(x => x.wilayahID == wlyhID && x.syarikatID == syrktID && x.negaraID == ngrID && x.deleted == false && x.fld_Purpose == "CHECKROLL").FirstOrDefault();
            host = getconnection.DataSource;
            catalog = getconnection.InitialCatalog;
            user = getconnection.userID;
            pass = getconnection.Password;

        }

        public MVC_SYSTEM_ModelsEstate GetConnectionMobile(int? wlyhID, int? syrktID, int? ngrID)
        {
            MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
            var getconnection = db.tblConnections.Where(x => x.wilayahID == wlyhID && x.syarikatID == syrktID && x.negaraID == ngrID && x.fld_Purpose == "CHECKROLL").FirstOrDefault();
            string host = getconnection.DataSource;
            string catalog = getconnection.InitialCatalog;
            string user = getconnection.userID;
            string pass = getconnection.Password;

            var dbr = new MVC_SYSTEM_ModelsEstate();
            return dbr.ConnectToSqlServerMobile(host, catalog, user, pass);
        }
    }
}