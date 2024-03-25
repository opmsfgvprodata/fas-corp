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
    public class SAPMasterAssetController : ApiController
    {
        MVC_SYSTEM_Models_SAPPUP db = new MVC_SYSTEM_Models_SAPPUP();

        [HttpPost]
        public JsonResult<UploadResult> Post(MASTERASSETITEM objData)
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
            var msg2 = "SAP MASTER ASSET";
            var msg3 = "";
            var msg4 = "";
            var parameter = "";
            var row = "";
            var field = "";
            var system = returnMessage.SystemName();
            var newRecordCount = 0;
            var updateRecordCount = 0;
            //var estateInfo = new ModelsSAPPUP.tbl_SAPOPMSCCLdgMapping();
            var masterAssetInfo = new ModelsSAPPUP.tbl_SAPMasterAssetMapping();

            try
            {
                    var masterAssetData = db.tbl_SAPMasterAsset.SingleOrDefault(x => x.fld_CostCenter == objData.ANLA_ANLN1 && x.fld_CompanyCode == objData.ANLA_BUKRS);

                    //sapPupConfig.SaveLog("SAP - MASTER ASSET", JsonConvert.SerializeObject(objData), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_LadangID, "SAP", "Inbound");

                    if (masterAssetData == null)
                    {
                        newRecordCount++;

                        //tbl_SAPMasterAsset newSAPMasterAsset = new tbl_SAPMasterAsset();

                        /*if (!String.IsNullOrEmpty(objData.ZINDDLTD))
                        {
                            newSAPCustomPUP.fld_Deleted = true;
                        }

                        else
                        {
                            newSAPCustomPUP.fld_Deleted = false;
                        }*/

                        masterAssetData.fld_CompanyCode = objData.ANLA_BUKRS.ToString().Trim();
                        masterAssetData.fld_AssetNo = objData.ANLA_ANLN1.Trim();
                        masterAssetData.fld_AssetSubNo = objData.ANLA_ANLN2.Trim();
                        masterAssetData.fld_AssetModel = objData.ANLA_TXT50.Trim();
                        masterAssetData.fld_InventoryNo = objData.ANLA_INVNR.Trim();
                        masterAssetData.fld_AssetQty = objData.ANLA_MENGE;
                        masterAssetData.fld_AssetUOM = objData.ANLA_MEINS;
                        masterAssetData.fld_CostCenter = objData.ANLA_KOSTL.Trim();
                        masterAssetData.fld_BusinessArea = objData.ANLA_GSBER;
                        masterAssetData.fld_AssetPlateNo = objData.ANLA_KFZKZ.Trim();
                        masterAssetData.fld_WBSNo = objData.ANLA_POSNR;
                        masterAssetData.fld_CapitalizedOn = objData.ANLA_AKTIV;
                        masterAssetData.fld_Acquitision = objData.ANLA_ZUGDT;
                        masterAssetData.fld_CreatedBy = objData.ANLA_ERNAM;
                        masterAssetData.fld_CreatedOn = objData.ANLA_ERDAT;
                        //newSAPMasterAsset.fld_IsSelected = false;
                        //newSAPMasterAsset.fld_CreatedBy = "SAP";
                        //newSAPMasterAsset.fld_CreatedDT = timezone.gettimezone();
                        //newSAPMasterAsset.fld_ModifiedBy = "SAP";
                        //newSAPMasterAsset.fld_ModifiedDT = timezone.gettimezone();

                        try
                        {
                            db.tbl_SAPMasterAsset.Add(masterAssetData);
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

                        masterAssetData.fld_AssetSubNo = objData.ANLA_ANLN2.Trim();
                        masterAssetData.fld_AssetModel = objData.ANLA_TXT50.Trim();
                        masterAssetData.fld_InventoryNo = objData.ANLA_INVNR.Trim();
                        masterAssetData.fld_AssetQty = objData.ANLA_MENGE;
                        masterAssetData.fld_AssetUOM = objData.ANLA_MEINS;
                        masterAssetData.fld_CostCenter = objData.ANLA_KOSTL.Trim();
                        masterAssetData.fld_BusinessArea = objData.ANLA_GSBER;
                        masterAssetData.fld_AssetPlateNo = objData.ANLA_KFZKZ.Trim();
                        masterAssetData.fld_WBSNo = objData.ANLA_POSNR;
                        masterAssetData.fld_CapitalizedOn = objData.ANLA_AKTIV;
                        masterAssetData.fld_Acquitision = objData.ANLA_ZUGDT;
                        masterAssetData.fld_CreatedBy = objData.ANLA_ERNAM;
                        masterAssetData.fld_CreatedOn = objData.ANLA_ERDAT;

                        /*if (!String.IsNullOrEmpty(objData.ZINDDLTD))
                        {
                            customData.fld_Deleted = true;
                        }

                        else
                        {
                            customData.fld_Deleted = false;
                        }*/

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

            if (masterAssetInfo != null)
            {
                //sapPupConfig.SaveLog("SAPMASTERASSET", JsonConvert.SerializeObject(UploadResult), estateInfo.fld_NegaraID, estateInfo.fld_SyarikatID, estateInfo.fld_WilayahID, estateInfo.fld_LadangID, "SAP", "Outbound");
            }
            return Json(UploadResult);
        }
    }
}
