using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsCustom;

namespace MVC_SYSTEM.Class
{
    public class GlobalFunction
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();

        public static class PropertyCopy
        {
            public static void Copy<TDest, TSource>(TDest destination, TSource source)
                where TSource : class
                where TDest : class
            {
                var destProperties = destination.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var sourceProperties = source.GetType()
                    .GetProperties()
                    .Where(x => x.CanRead && x.CanWrite && !x.GetGetMethod().IsVirtual);
                var copyProperties = sourceProperties.Join(destProperties, x => x.Name, y => y.Name, (x, y) => x);
                foreach (var sourceProperty in copyProperties)
                {
                    var prop = destProperties.FirstOrDefault(x => x.Name == sourceProperty.Name);
                    prop.SetValue(destination, sourceProperty.GetValue(source));
                }
            }
        }

        public void GenerateLHDNSiriNo(int? year)
        {
            if (year != null)
            {
                Connection connection = new Connection();
                var regionData = db.vw_NSWL.Where(x => x.fld_Deleted_W == false).Select(s => new { s.fld_WilayahID, s.fld_SyarikatID, s.fld_NegaraID }).Distinct().ToList();
                List<CusMod_EAWorkerInfo> eAWorkerInfoList = new List<CusMod_EAWorkerInfo>();
                foreach (var region in regionData)
                {
                    var db = connection.GetConnectionMobile(region.fld_WilayahID, region.fld_SyarikatID, region.fld_NegaraID);

                    var workerTaxInfo = db.tbl_TaxWorkerInfo.Join(db.tbl_Pkjmast, workerTax => workerTax.fld_NopkjPermanent, workerInfo => workerInfo.fld_NopkjPermanent, (workerTax, workerInfo) => new { workerTax, workerInfo })
                        .Join(db.tbl_GajiBulanan, x => x.workerTax.fld_NopkjPermanent, gaji => gaji.fld_NoPkjPermanent, (workerTax, gaji) => new { workerTax, gaji })
                        .Join(db.tbl_ByrCarumanTambahan, caruman => caruman.gaji.fld_ID, carumanTmbh => carumanTmbh.fld_GajiID, (caruman, carumanTmbh) => new { caruman, carumanTmbh })
                        .Where(x => x.carumanTmbh.fld_Year == year && x.carumanTmbh.fld_KodSubCaruman == "PCB02" && x.caruman.workerTax.workerTax.fld_Year == year).ToList();

                    eAWorkerInfoList.AddRange(workerTaxInfo.Select(s => new CusMod_EAWorkerInfo { WorkerNo = s.caruman.workerTax.workerInfo.fld_NopkjPermanent, WorkerName = s.caruman.workerTax.workerInfo.fld_Nama }).Distinct().ToList());
                }
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE tbl_EAWorkerSiriNo");
                var tbl_EAWorkerSiriNo = new List<tbl_EAWorkerSiriNo>();
                var no = 1;
                var workerTaxData = eAWorkerInfoList.Select(s => new { s.WorkerName, s.WorkerNo }).Distinct().ToList();
                foreach (var worker in workerTaxData.OrderBy(o => o.WorkerName).ToList())
                {
                    var noSiri = no.ToString().PadLeft(5, '0');
                    tbl_EAWorkerSiriNo.Add(new tbl_EAWorkerSiriNo { fld_SiriNo = noSiri, fld_WorkerNo = worker.WorkerNo });
                    no++;
                }
                db.tbl_EAWorkerSiriNo.AddRange(tbl_EAWorkerSiriNo);
                db.SaveChanges();
            }
        }
    }
}