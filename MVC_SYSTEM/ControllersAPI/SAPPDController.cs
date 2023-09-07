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
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsSAPPUP;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MVC_SYSTEM.ControllersAPI
{
    public class SAPPDController : ApiController
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();
        MVC_SYSTEM_ModelsCorporate db2 = new MVC_SYSTEM_ModelsCorporate();

        [HttpPost]
        public JsonResult<UploadResult> Post(PDITEM objData)
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
            var msg2 = "PS";
            var msg3 = "";
            var msg4 = "";
            var parameter = "";
            var row = "";
            var field = "";
            var system = returnMessage.SystemName();
            var newRecordCount = 0;
            var updateRecordCount = 0;
            var estateInfo = new ModelsSAPPUP.tbl_SAPOPMSCCLdgMapping();
            string Block = "";
            string StartingWbs = "";
            string FullWBSElement = "";
            int Loop = 1;
            int Loop1 = 1;
            string KodKategoriAktvt = "";
            bool ReplantingChecking = false;

            try
            {
                Loop1 = 1;
                //string[] PROJ_PSPIDs = objData.PROJ_PSPID.Trim().Split('.');
                //foreach (string PROJ_PSPID in PROJ_PSPIDs)
                //{
                //    if (Loop == 2)
                //    {
                //        objData.PROJ_PSPID = PROJ_PSPID;
                //    }

                //    Loop++;
                //}

                estateInfo =
                    db.tbl_SAPOPMSCCLdgMapping.SingleOrDefault(x => x.fld_CostCenter == objData.PRPS_FKSTL);

                if (estateInfo == null)
                {
                    msg3 = "Unable to find matching estate";
                }
                else
                {
                    Loop = 1;
                    Block = "";
                    StartingWbs = "";
                    FullWBSElement = "";
                    FullWBSElement = objData.PRPS_POSID.Trim().Substring(0, 17);
                    //string[] words = objData.PRPS_POSID.Trim().Split('.');
                    //foreach (string word in words)
                    //{
                    //    if (Loop == 1)
                    //    {
                    //        ReplantingChecking = word == "C" ? true : false;
                    //    }

                    //    if (Loop == 4)
                    //    {
                    //        Block = word;
                    //    }
                    //    Loop++;
                    //}

                    //if (!ReplantingChecking)
                    //{
                    //    FullWBSElement = StartingWbs + "." + Block;
                    //}
                    //else
                    //{
                    //    FullWBSElement = objData.PRPS_POSID.Trim().Substring(0, 18);
                    //}

                    KodKategoriAktvt = db2.tbl_KategoriAktiviti.Where(x => x.fld_NegaraID == estateInfo.fld_NegaraID && x.fld_SyarikatID == estateInfo.fld_SyarikatID && x.fld_KodJnsAktvt == "E" && x.fld_Kategori.Trim() == objData.PRPS_POST1.Trim()).Select(s => s.fld_KodKategori).FirstOrDefault();

                    var GetNN = objData.CAUFVD_AUFNR.Trim().ToString().PadLeft(10, '0');

                    var PDData = db.tbl_SAPPDPUP.SingleOrDefault(x =>
                        x.fld_NegaraID == estateInfo.fld_NegaraID && x.fld_SyarikatID == estateInfo.fld_SyarikatID &&
                        x.fld_WilayahID == estateInfo.fld_WilayahID && x.fld_LadangID == estateInfo.fld_LadangID &&
                        x.fld_ProjectDefinition == objData.PROJ_PSPID && x.fld_WBSElement == objData.PRPS_POSID &&
                        x.fld_WBSCode == FullWBSElement && x.fld_KodKategori == KodKategoriAktvt &&
                        x.fld_ActivityCode == objData.AFVGD_VORNR && x.fld_CostCenter == objData.PRPS_FKSTL);

                    sapPupConfig.SaveLog("SAPPDPUP", JsonConvert.SerializeObject(objData), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_LadangID, "SAP", "Inbound");

                    if (PDData == null)
                    {
                        newRecordCount++;

                        ModelsSAPPUP.tbl_SAPPDPUP newSAPPDPUP = new ModelsSAPPUP.tbl_SAPPDPUP();

                        if (!String.IsNullOrEmpty(objData.STATUS))
                        {
                            newSAPPDPUP.fld_Deleted = true;
                        }

                        else
                        {
                            newSAPPDPUP.fld_Deleted = false;
                        }


                        newSAPPDPUP.fld_ProjectDefinition = objData.PROJ_PSPID.Trim();
                        newSAPPDPUP.fld_ProjectDesc = objData.PROJ_POST1.Trim();
                        newSAPPDPUP.fld_WBSElement = objData.PRPS_POSID.Trim();
                        newSAPPDPUP.fld_WBSDesc = objData.PRPS_POST1.Trim();
                        //newSAPPDPUP.fld_NetworkNo = string.Format("{0:000000000000}", objData.CAUFVD_AUFNR.Trim()); ;
                        newSAPPDPUP.fld_NetworkNo = GetNN;
                        newSAPPDPUP.fld_ActivityCode = objData.AFVGD_VORNR.Trim();
                        newSAPPDPUP.fld_ActivityDesc = objData.AFVGM_LTXA1.Trim();
                        newSAPPDPUP.fld_NegaraID = estateInfo.fld_NegaraID;
                        newSAPPDPUP.fld_SyarikatID = estateInfo.fld_SyarikatID;
                        newSAPPDPUP.fld_WilayahID = estateInfo.fld_WilayahID;
                        newSAPPDPUP.fld_LadangID = estateInfo.fld_LadangID;
                        newSAPPDPUP.fld_Deleted = false;
                        newSAPPDPUP.fld_IsSelected = false;
                        newSAPPDPUP.fld_CreatedBy = "SAP";
                        newSAPPDPUP.fld_CreatedDT = timezone.gettimezone();
                        newSAPPDPUP.fld_ModifiedBy = "SAP";
                        newSAPPDPUP.fld_ModifiedDT = timezone.gettimezone();
                        newSAPPDPUP.fld_WBSCode = FullWBSElement;
                        newSAPPDPUP.fld_KodKategori = KodKategoriAktvt;
                        newSAPPDPUP.fld_CostCenter = objData.PRPS_FKSTL;

                        //if (objData.PRPS_POSID.Length == 18)
                        //{
                        //    newSAPPDPUP.fld_WBSCode = objData.PRPS_POSID.Substring(0, 12);
                        //    newSAPPDPUP.fld_WorkActivity = objData.PRPS_POSID.Substring(14, 1);
                        //    newSAPPDPUP.fld_ActivityType = objData.PRPS_POSID.Substring(16, 2);
                        //}

                        //else
                        //{
                        //    newSAPPDPUP.fld_WBSCode = objData.PRPS_POSID.Substring(0, 12);
                        //    newSAPPDPUP.fld_WorkActivity = objData.PRPS_POSID.Substring(14, 1);
                        //}

                        try
                        {
                            db.tbl_SAPPDPUP.Add(newSAPPDPUP);
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

                        PDData.fld_NetworkNo = GetNN;
                        PDData.fld_WBSDesc = objData.PRPS_POST1.Trim();
                        PDData.fld_ActivityDesc = objData.AFVGM_LTXA1.Trim();
                        PDData.fld_ModifiedBy = "SAP";
                        PDData.fld_ModifiedDT = timezone.gettimezone();

                        if (!String.IsNullOrEmpty(objData.STATUS))
                        {
                            PDData.fld_Deleted = true;
                        }

                        else
                        {
                            PDData.fld_Deleted = false;
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
                sapPupConfig.SaveLog("SAPPDPUP", JsonConvert.SerializeObject(UploadResult), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_ID, "SAP", "Outbound");
            }

            return Json(UploadResult);
        }
    }
}
