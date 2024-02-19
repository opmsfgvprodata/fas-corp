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
    public class SAPCUSTOMController : ApiController
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();

        [HttpPost]
        public JsonResult<UploadResult> Post(CUSTOMITEM objData)
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
            var msg2 = "PS CUSTOM";
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
                short Loop = 1;
                string StationArea = "";
                string StationPkt = "";
                string[] WBSCodes = objData.ZWBSCO.Trim().Split('.');
                foreach (string WBSCode in WBSCodes)
                {
                    if (Loop == 4)
                    {
                        StationArea = WBSCode;
                    }

                    if (Loop == 5)
                    {
                        StationPkt = WBSCode;
                    }
                    Loop++;
                }
                //shah - mintak semak dgn sap data ZBUKRS, ZDIVID, ZBLKID, ZCOSTC - 28/01/2020
                var CostCenter = "0" + objData.ZBUKRS.ToString().Substring(0,2) + objData.ZDIVID.Trim() + objData.ZBLKID.Trim() + objData.ZCOSTC;
                //shah - mintak semak dgn sap data ZBUKRS, ZDIVID, ZBLKID, ZCOSTC - 28/01/2020
                estateInfo =
                    db.tbl_SAPOPMSCCLdgMapping.SingleOrDefault(x => x.fld_CostCenter == CostCenter);

                if (estateInfo == null)
                {
                    msg3 = "Unable to find matching company code";
                }
                else
                {
                    var customData = db.tbl_SAPCUSTOMPUP.SingleOrDefault(x =>
                    x.fld_NegaraID == estateInfo.fld_NegaraID && x.fld_SyarikatID == estateInfo.fld_SyarikatID &&
                    x.fld_WilayahID == estateInfo.fld_WilayahID && x.fld_LadangID == estateInfo.fld_LadangID &&
                    x.fld_WilayahCode == objData.ZREGIO && x.fld_PktUtama == objData.ZDIVID &&
                    x.fld_Blok == objData.ZBLKID);

                    sapPupConfig.SaveLog("SAPCUSTOMPUP", JsonConvert.SerializeObject(objData), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_LadangID, "SAP", "Inbound");

                    if (customData == null)
                    {
                        newRecordCount++;

                        tbl_SAPCUSTOMPUP newSAPCustomPUP = new tbl_SAPCUSTOMPUP();

                        if (!String.IsNullOrEmpty(objData.ZINDDLTD))
                        {
                            newSAPCustomPUP.fld_Deleted = true;
                        }

                        else
                        {
                            newSAPCustomPUP.fld_Deleted = false;
                        }

                        //idzham tambah
                        newSAPCustomPUP.fld_Year = objData.ZGJAHR;

                        newSAPCustomPUP.fld_CompanyCode = objData.ZBUKRS.ToString().Trim();
                        newSAPCustomPUP.fld_WilayahCode = objData.ZREGIO.Trim();
                        newSAPCustomPUP.fld_PktUtama = objData.ZDIVID.Trim();
                        newSAPCustomPUP.fld_Blok = objData.ZBLKID.Trim();
                        newSAPCustomPUP.fld_JnsTnmn = objData.ZCROPT.Trim();
                        newSAPCustomPUP.fld_LsPktUtama = objData.ZHECTA;
                        newSAPCustomPUP.fld_DirianPokok = objData.ZSPHEC;
                        newSAPCustomPUP.fld_WBSCode = objData.ZWBSCO.Trim();
                        newSAPCustomPUP.fld_TahunTnm = objData.ZYEARP;
                        newSAPCustomPUP.fld_StatusTnmn = objData.ZCROPC.Trim();
                        newSAPCustomPUP.fld_LuasBerhasil = objData.ZLSKBH;
                        newSAPCustomPUP.fld_NegaraID = estateInfo.fld_NegaraID;
                        newSAPCustomPUP.fld_SyarikatID = estateInfo.fld_SyarikatID;
                        newSAPCustomPUP.fld_WilayahID = estateInfo.fld_WilayahID;
                        newSAPCustomPUP.fld_LadangID = estateInfo.fld_LadangID;
                        newSAPCustomPUP.fld_IsSelected = false;
                        newSAPCustomPUP.fld_CreatedBy = "SAP";
                        newSAPCustomPUP.fld_CreatedDT = timezone.gettimezone();
                        newSAPCustomPUP.fld_ModifiedBy = "SAP";
                        newSAPCustomPUP.fld_ModifiedDT = timezone.gettimezone();

                        try
                        {
                            db.tbl_SAPCUSTOMPUP.Add(newSAPCustomPUP);
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
                        //idzham tambah
                        customData.fld_Year = objData.ZGJAHR;

                        customData.fld_LsPktUtama = objData.ZHECTA;
                        customData.fld_DirianPokok = objData.ZSPHEC;
                        customData.fld_StatusTnmn = objData.ZCROPC.Trim();
                        customData.fld_LuasBerhasil = objData.ZLSKBH;
                        customData.fld_ModifiedBy = "SAP";
                        customData.fld_ModifiedDT = timezone.gettimezone();

                        if (!String.IsNullOrEmpty(objData.ZINDDLTD))
                        {
                            customData.fld_Deleted = true;
                        }

                        else
                        {
                            customData.fld_Deleted = false;
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
                sapPupConfig.SaveLog("SAPCUSTOMPUP", JsonConvert.SerializeObject(UploadResult), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_LadangID, "SAP", "Outbound");
            }
            return Json(UploadResult);
        }
    }
}
