using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.ModelsMobileAPI.ModelsCustom
{
    public class tbl_ActivityType
    {
        [Key]
        public int fld_ActivityTypeID { get; set; }

        public string fld_ActivityTypeCode { get; set; }

        public string fld_Desc { get; set; }

        public short fld_FormFlag { get; set; }

        public int fld_NegaraID { get; set; }

        public int fld_SyarikatID { get; set; }

        public int fld_WilayahID { get; set; }

        public int fld_LadangID { get; set; }
    }
}