using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsMobileAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;
using MVC_SYSTEM.Security;
using MVC_SYSTEM.Class;
using System.Web.Script.Serialization;
using System.Web.Http.Cors;

namespace MVC_SYSTEM.ControllersMobileAPI
{
    public class LoginController : ApiController
    {
        [HttpPost]
        public JsonResult<MasterDataLogin> Post([FromBody] LoginForm LoginForm)
        {
            MasterDataLogin MasterData = new MasterDataLogin();
            errorlog geterror = new errorlog();
            GetMasterData GetMasterData = new GetMasterData();
            LoginResult LoginResult = new LoginResult();
            int ID = 1;
            try
            {
                MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
                EncryptDecrypt EncryptDecrypt = new EncryptDecrypt();
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(LoginForm);
                geterror.testlog(json, "Login");
                string password = "";
                password = EncryptDecrypt.Encrypt(LoginForm.Password);
                var user = db.tblUsers.Where(u => u.fldUserName == LoginForm.Username.ToUpper() && u.fldUserPassword == password && u.fldDeleted == false).SingleOrDefault();
                if (user != null)
                {
                    var ladangdetail = db.tbl_Ladang.Where(x => x.fld_ID == user.fldLadangID && x.fld_WlyhID == user.fldWilayahID).FirstOrDefault();
                    LoginResult.fld_ProfileID = ID;
                    LoginResult.fld_KmplnSyrktID = user.fld_KmplnSyrktID;
                    LoginResult.fld_NegaraID = user.fldNegaraID;
                    LoginResult.fld_SyarikatID = user.fldSyarikatID;
                    LoginResult.fld_WilayahID = user.fldWilayahID;
                    LoginResult.fld_LadangID = user.fldLadangID;
                    LoginResult.fld_KodLadang = ladangdetail.fld_LdgCode;
                    LoginResult.fld_NamaLadang = ladangdetail.fld_LdgName;
                    LoginResult.LoginStatus = 1; // success
                    LoginResult.RemarkStatus = "Successfully Login";

                    MasterData.LoginResult = LoginResult;

                    MasterData.tbl_Users = GetMasterData.tbl_Users(user.fld_KmplnSyrktID.Value, user.fldNegaraID.Value, user.fldSyarikatID.Value, user.fldWilayahID.Value, user.fldLadangID.Value);
                    MasterData.tbl_Division = GetMasterData.tbl_Division(user.fld_KmplnSyrktID.Value, user.fldNegaraID.Value, user.fldSyarikatID.Value, user.fldWilayahID.Value, user.fldLadangID.Value);
                }
                else
                {
                    LoginResult.fld_ProfileID = ID;
                    LoginResult.fld_KmplnSyrktID = 0;
                    LoginResult.fld_NegaraID = 0;
                    LoginResult.fld_SyarikatID = 0;
                    LoginResult.fld_WilayahID = 0;
                    LoginResult.fld_LadangID = 0;
                    LoginResult.fld_KodLadang = "";
                    LoginResult.fld_NamaLadang = "";
                    LoginResult.LoginStatus = 2; // wrong username or password
                    LoginResult.RemarkStatus = "Wrong username or password";

                    MasterData.LoginResult = LoginResult;
                }
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                LoginResult.fld_ProfileID = ID;
                LoginResult.fld_KmplnSyrktID = 0;
                LoginResult.fld_NegaraID = 0;
                LoginResult.fld_SyarikatID = 0;
                LoginResult.fld_WilayahID = 0;
                LoginResult.fld_LadangID = 0;
                LoginResult.fld_KodLadang = "";
                LoginResult.fld_NamaLadang = "";
                LoginResult.LoginStatus = 0; // error
                LoginResult.RemarkStatus = "Error please contact ashahri.as@feldagobal.com";

                MasterData.LoginResult = LoginResult;
            }

            return Json(MasterData);
        }
    }
}
