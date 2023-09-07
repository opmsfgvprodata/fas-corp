using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.ModelsSAPPUP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MVC_SYSTEM.ControllersAPI
{
    public class SAPCUSTOMERController : ApiController
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();

        [HttpPost]
        public JsonResult<UploadResult> Post(CUSTOMERITEM objData)
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
            var msg2 = "CUSTOMER";
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
                estateInfo =
                    db.tbl_Syarikat.SingleOrDefault(x => x.fld_SAPComCode == objData.KNB1_BUKRS.ToString() && x.fld_Deleted == false);

                if (estateInfo == null)
                {
                    msg3 = "Unable to find matching company code";
                }
                else
                {
                    var customerData = db.tbl_SAPCustomerPUP.SingleOrDefault(x =>
                    x.fld_NegaraID == estateInfo.fld_NegaraID && x.fld_SyarikatID == estateInfo.fld_SyarikatID &&
                    x.fld_WilayahID == 0 && x.fld_LadangID == 0 &&
                    x.fld_CustomerCode == objData.KNB1_KUNNR);

                    sapPupConfig.SaveLog("SAPCUSTOMERPUP", JsonConvert.SerializeObject(objData), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, 0, 0, "SAP", "Inbound");

                    if (customerData == null)
                    {
                        newRecordCount++;

                        tbl_SAPCustomerPUP newSAPCustomerPUP = new tbl_SAPCustomerPUP();

                        if (!String.IsNullOrEmpty(objData.STATUS))
                        {
                            newSAPCustomerPUP.fld_Deleted = true;
                        }

                        else
                        {
                            newSAPCustomerPUP.fld_Deleted = false;
                        }

                        newSAPCustomerPUP.fld_CompanyCode = objData.KNB1_BUKRS.ToString().Trim();
                        newSAPCustomerPUP.fld_CustomerCode = objData.KNB1_KUNNR.Trim().PadLeft(10, '0');
                        newSAPCustomerPUP.fld_CustomerName = objData.KNA1_NAME1.Trim();
                        newSAPCustomerPUP.fld_NegaraID = estateInfo.fld_NegaraID;
                        newSAPCustomerPUP.fld_SyarikatID = estateInfo.fld_SyarikatID;
                        newSAPCustomerPUP.fld_WilayahID = 0;
                        newSAPCustomerPUP.fld_LadangID = 0;
                        newSAPCustomerPUP.fld_Deleted = false;
                        newSAPCustomerPUP.fld_IsSelected = false;
                        newSAPCustomerPUP.fld_CreatedBy = "SAP";
                        newSAPCustomerPUP.fld_CreatedDT = timezone.gettimezone();
                        newSAPCustomerPUP.fld_ModifiedBy = "SAP";
                        newSAPCustomerPUP.fld_ModifiedDT = timezone.gettimezone();

                        try
                        {
                            db.tbl_SAPCustomerPUP.Add(newSAPCustomerPUP);
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

                        customerData.fld_CustomerName = objData.KNA1_NAME1.Trim();
                        customerData.fld_ModifiedBy = "SAP";
                        customerData.fld_ModifiedDT = timezone.gettimezone();

                        if (!String.IsNullOrEmpty(objData.STATUS))
                        {
                            customerData.fld_Deleted = true;
                        }

                        else
                        {
                            customerData.fld_Deleted = false;
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
                sapPupConfig.SaveLog("SAPCUSTOMERPUP", JsonConvert.SerializeObject(UploadResult), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, 0, 0, "SAP", "Outbound");
            }

            return Json(UploadResult);
        }
    }
}
