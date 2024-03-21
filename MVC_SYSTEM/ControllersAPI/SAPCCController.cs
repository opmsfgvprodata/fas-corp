using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCustom;
using MVC_SYSTEM.ModelsEstate;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using iTextSharp.text.pdf.hyphenation;
using MVC_SYSTEM.ModelsSAPPUP;

namespace MVC_SYSTEM.ControllersAPI
{
    public class SAPCCController : ApiController
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();

        [HttpPost]
        public JsonResult<UploadResult> Post(CCITEM objData)
        {
            errorlog geterror = new errorlog();
            UploadResult UploadResult = new UploadResult();
            SAPPUPMessage returnMessage = new SAPPUPMessage();
            ChangeTimeZone timezone = new ChangeTimeZone();
            SAPPUPConfig sapPupConfig = new SAPPUPConfig();

            var result = "";
            var LogReturn = "";
            var type = "";
            var id = "";
            var number = "";
            var message = "";
            var logNo = "";
            var logMsgNo = "";
            var msg1 = "";
            var msg2 = "COST CENTER";
            var msg3 = "";
            var msg4 = "";
            var parameter = "";
            var row = "";
            var field = "";
            var system = returnMessage.SystemName();
            var newRecordCount = 0;
            var updateRecordCount = 0;
            var estateInfo = new ModelsSAPPUP.tbl_SAPOPMSCCLdgMapping();

            try
            {
                estateInfo =
                    db.tbl_SAPOPMSCCLdgMapping.SingleOrDefault(x => x.fld_CostCenter == objData.CSKS_KOSTL.Trim());

                if (estateInfo == null)
                {
                    msg3 = "Unable to find matching company code";
                }
                else
                {
                    var CCData = db.tbl_SAPCCPUP.SingleOrDefault(x =>
                    x.fld_NegaraID == estateInfo.fld_NegaraID && x.fld_SyarikatID == estateInfo.fld_SyarikatID &&
                    x.fld_CostCenter == objData.CSKS_KOSTL);

                    sapPupConfig.SaveLog("SAPCCPUP", JsonConvert.SerializeObject(objData), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_LadangID, "SAP", "Inbound");

                    if (CCData == null)
                    {
                        newRecordCount++;

                        tbl_SAPCCPUP newSAPCCPUP = new tbl_SAPCCPUP();

                        if (!String.IsNullOrEmpty(objData.CSKS_BKZKP))
                        {
                            newSAPCCPUP.fld_Deleted = true;
                        }

                        else
                        {
                            newSAPCCPUP.fld_Deleted = false;
                        }

                        newSAPCCPUP.fld_CompanyCode = objData.CSKS_BUKRS.ToString().Trim();
                        newSAPCCPUP.fld_CostCenter = objData.CSKS_KOSTL.Trim();
                        newSAPCCPUP.fld_CostCenterDesc = objData.CSKT_LTEXT.Trim();
                        newSAPCCPUP.fld_CCIOStatus = 1; // 1 = CC
                        newSAPCCPUP.fld_NegaraID = estateInfo.fld_NegaraID;
                        newSAPCCPUP.fld_SyarikatID = estateInfo.fld_SyarikatID;
                        newSAPCCPUP.fld_WilayahID = estateInfo.fld_WilayahID;
                        newSAPCCPUP.fld_LadangID = estateInfo.fld_LadangID;
                        newSAPCCPUP.fld_Deleted = false;
                        newSAPCCPUP.fld_IsSelected = false;
                        newSAPCCPUP.fld_CreatedBy = "SAP";
                        newSAPCCPUP.fld_CreatedDT = timezone.gettimezone();
                        newSAPCCPUP.fld_ModifiedBy = "SAP";
                        newSAPCCPUP.fld_ModifiedDT = timezone.gettimezone();

                        try
                        {
                            db.tbl_SAPCCPUP.Add(newSAPCCPUP);
                            db.SaveChanges();

                            type = returnMessage.SuccessCode();
                            msg1 = returnMessage.CreateDataSuccessMessage(newRecordCount);
                            message = returnMessage.SuccessMessage();
                        }

                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    message = ve.PropertyName + " " + ve.ErrorMessage;
                                }
                            }

                            message = message + message;
                        }
                    }

                    else
                    {
                        updateRecordCount++;

                        CCData.fld_CostCenterDesc = objData.CSKT_LTEXT.Trim();
                        CCData.fld_ModifiedBy = "SAP";
                        CCData.fld_ModifiedDT = timezone.gettimezone();
                        //tambahan
                        CCData.fld_CompanyCode = objData.CSKS_BUKRS.ToString().Trim();
                        CCData.fld_CostCenter = objData.CSKS_KOSTL.Trim();
                        CCData.fld_CostCenterDesc = objData.CSKT_LTEXT.Trim();

                        if (!String.IsNullOrEmpty(objData.CSKS_BKZKP))
                        {
                            CCData.fld_Deleted = true;
                        }

                        else
                        {
                            CCData.fld_Deleted = false;
                        }

                        try
                        {
                            db.SaveChanges();

                            type = returnMessage.SuccessCode();
                            msg1 = returnMessage.UpdateDataSuccessMessage(updateRecordCount);
                            message = returnMessage.SuccessMessage();
                        }

                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    message = ve.PropertyName + " " + ve.ErrorMessage;
                                }
                            }

                            message = message + message;
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());

                type = returnMessage.ErrorCode();
                message = ex.Message;
            }

            UploadResult.TYPE = type;
            UploadResult.ID = id;
            UploadResult.NUMBER = number;
            UploadResult.MESSAGE = message;
            UploadResult.LOG_NO = logNo;
            UploadResult.LOG_MSG_NO = logMsgNo;
            UploadResult.MESSAGE_V1 = msg1;
            UploadResult.MESSAGE_V2 = msg2;
            UploadResult.MESSAGE_V3 = msg3;
            UploadResult.MESSAGE_V4 = msg4;
            UploadResult.PARAMETER = parameter;
            UploadResult.ROW = row;
            UploadResult.FIELD = field;
            UploadResult.SYSTEM = system;

            if (estateInfo != null)
            {
                sapPupConfig.SaveLog("SAPCCPUP", JsonConvert.SerializeObject(UploadResult), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_LadangID, "SAP", "Outbound");
            }
            return Json(UploadResult);
        }
    }
}