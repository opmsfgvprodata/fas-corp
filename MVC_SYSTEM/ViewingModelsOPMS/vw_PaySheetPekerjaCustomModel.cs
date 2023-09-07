using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.Models;

namespace MVC_SYSTEM.ViewingModelsOPMS
{
    public class vw_PaySheetPekerjaCustomModel
    {
        public vw_PaySheetPekerja PaySheetPekerja { get; set; }
        public List<CarumanTambahanCustomModel> CarumanTambahan { get; set; }
    }
}