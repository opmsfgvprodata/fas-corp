using System.Web.Mvc;
using MVC_SYSTEM.App_LocalResources;

namespace MVC_SYSTEM.ModelsCorporate
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class tbl_MapGL
    {
        [Key]
        public int fld_ID { get; set; }


        //[StringLength(10)] //Commented by Shazana on 5/11/2021
        [StringLength(4)]//Commented by Shazana on 5/11/2021
        public string fld_KodAktvt { get; set; }

        [StringLength(10)]
        public string fld_KodGL { get; set; }

        [StringLength(2)]//Commented by Shazana on 5/11/2021
        //[StringLength(10)] //Commented by Shazana on 5/11/2021
        public string fld_Paysheet { get; set; }

        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public int? fld_WilayahID { get; set; }

        public int? fld_LadangID { get; set; }

        public bool? fld_Deleted { get; set; }
    }

    public partial class tbl_MapGLViewModelCreate
    {
        [Key]
        public IEnumerable<int> fld_ID { get; set; }

        //[StringLength(10)]
        public List<string> fld_KodAktvt { get; set; }

        [StringLength(10)]
        public string fld_Paysheet { get; set; }

        [StringLength(10)]
        public string fld_KodGL { get; set; }
        public int? fld_NegaraID { get; set; }

        public int? fld_SyarikatID { get; set; }

        public bool? fld_Deleted { get; set; }
    }
}
