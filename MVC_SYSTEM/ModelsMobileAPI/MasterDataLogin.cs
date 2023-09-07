using MVC_SYSTEM.ModelsMobileAPI.ModelsCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI
{
    public class MasterDataLogin
    {
        public LoginResult LoginResult { get; set; }
        public List<tbl_Users> tbl_Users { get; set; }
        public List<tbl_Division> tbl_Division { get; set; }
    }
}