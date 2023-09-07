using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Class;

namespace MVC_SYSTEM.ModelsSAPPUP
{
    public class SAPPUPConfig
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();

        ChangeTimeZone timezone = new ChangeTimeZone();

        public void SaveLog(string Originator, string Message, int? NegaraID, int? SyarikatID, int? WilayahID, int? LadangID, string CreatedBy, string Action)
        {
            tbl_SAPLogPUP sapLogPup = new tbl_SAPLogPUP();

            sapLogPup.fld_Originator = Originator;
            sapLogPup.fld_Message = Message;
            sapLogPup.fld_NegaraID = NegaraID;
            sapLogPup.fld_SyarikatID = SyarikatID;
            sapLogPup.fld_WilayahID = WilayahID;
            sapLogPup.fld_LadangID = LadangID;
            sapLogPup.fld_CreatedBy = CreatedBy;
            sapLogPup.fld_CreatedDT = timezone.gettimezone();
            sapLogPup.fld_Action = Action;

            db.tbl_SAPLogPUP.Add(sapLogPup);
            db.SaveChanges();
        }

        
    }
}