using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_Users
    {
        [Key]
        public int fld_UserID { get; set; }

        public string fld_UserName { get; set; }

        public string fld_UserPassword { get; set; }

        public int? fld_RoleID { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }
    }
}