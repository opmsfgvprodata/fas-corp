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
using MVC_SYSTEM.ModelsSAPPUP;
using Microsoft.AspNet.SignalR.Json;

namespace MVC_SYSTEM.ControllersAPI
{
    public class SAPGLController : ApiController
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();

        [HttpPost]
        public JsonResult<UploadResult> Post(GLITEM objData)
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
            var msg2 = "GL";
            var msg3 = "";
            var msg4 = "";
            var parameter = "";
            var row = "";
            var field = "";
            var system = returnMessage.SystemName();
            var newRecordCount = 0;
            var updateRecordCount = 0;
            var estateInfo = new ModelsSAPPUP.tbl_Syarikat();

            try
            {

                //JObject rss = JObject.Parse(objData);
                //string rssTitle = (string)rss["channel"]["title"];

                estateInfo =
                    db.tbl_Syarikat.SingleOrDefault(x => x.fld_SAPComCode == objData.SKB1_BUKRS.ToString() && x.fld_Deleted == false);

                if (estateInfo == null)
                {
                    msg3 = "Unable to find matching company code";
                }
                else
                {
                    var GLData = db.tbl_SAPGLPUP.SingleOrDefault(x =>
                    x.fld_NegaraID == estateInfo.fld_NegaraID && x.fld_SyarikatID == estateInfo.fld_SyarikatID &&
                    x.fld_WilayahID == 0 && x.fld_LadangID == 0 &&
                    x.fld_GLCode == objData.SKB1_SAKNR);

                    sapPupConfig.SaveLog("SAPGLPUP", JsonConvert.SerializeObject(objData), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, 0, 0, "SAP", "Inbound");

                    if (GLData == null)
                    {
                        newRecordCount++;

                        tbl_SAPGLPUP newSAPGLPUP = new tbl_SAPGLPUP();

                        if (!String.IsNullOrEmpty(objData.SKB1_XLOEB))
                        {
                            newSAPGLPUP.fld_Deleted = true;
                        }

                        else
                        {
                            newSAPGLPUP.fld_Deleted = false;
                        }

                        newSAPGLPUP.fld_CompanyCode = objData.SKB1_BUKRS.ToString().Trim();
                        newSAPGLPUP.fld_GLCode = objData.SKB1_SAKNR.Trim().PadLeft(10, '0');
                        newSAPGLPUP.fld_GLDesc = objData.SKAT_TXT50.Trim();
                        newSAPGLPUP.fld_NegaraID = estateInfo.fld_NegaraID;
                        newSAPGLPUP.fld_SyarikatID = estateInfo.fld_SyarikatID;
                        newSAPGLPUP.fld_WilayahID = 0;
                        newSAPGLPUP.fld_LadangID = 0;
                        newSAPGLPUP.fld_Deleted = false;
                        newSAPGLPUP.fld_IsSelected = false;
                        newSAPGLPUP.fld_CreatedBy = "SAP";
                        newSAPGLPUP.fld_CreatedDT = timezone.gettimezone();
                        newSAPGLPUP.fld_ModifiedBy = "SAP";
                        newSAPGLPUP.fld_ModifiedDT = timezone.gettimezone();

                        try
                        {
                            db.tbl_SAPGLPUP.Add(newSAPGLPUP);
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

                        GLData.fld_GLDesc = objData.SKAT_TXT50.Trim();
                        GLData.fld_ModifiedBy = "SAP";
                        GLData.fld_ModifiedDT = timezone.gettimezone();

                        if (!String.IsNullOrEmpty(objData.SKB1_XLOEB))
                        {
                            GLData.fld_Deleted = true;
                        }

                        else
                        {
                            GLData.fld_Deleted = false;
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
                sapPupConfig.SaveLog("SAPGLPUP", JsonConvert.SerializeObject(UploadResult), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, 0, 0, "SAP", "Outbound");
            }

            return Json(UploadResult);
        }
    }
}
