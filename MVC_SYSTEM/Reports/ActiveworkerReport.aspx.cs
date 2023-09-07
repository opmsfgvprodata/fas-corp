using Microsoft.Reporting.WebForms;
using MVC_SYSTEM.ModelsSP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MVC_SYSTEM.Reports
{
    public partial class ActiveworkerReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();
                int NegaraID = int.Parse(Request.QueryString["NegaraID"]);
                int SyarikatID = int.Parse(Request.QueryString["SyarikatID"]);
                int WilayahID = int.Parse(Request.QueryString["WilayahID"]);
                int LadangID = int.Parse(Request.QueryString["LadangID"]);
                int Status = int.Parse(Request.QueryString["Status"]);
                int UserID = int.Parse(Request.QueryString["UserID"]);

                List<sp_RptRumKedKepPekLad_Result> resultreport = new List<sp_RptRumKedKepPekLad_Result>();

                if (WilayahID == 0)
                {
                    dbSP.SetCommandTimeout(120);
                    resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahID, LadangID, Status, UserID).ToList();
                }
                else
                {
                    if (LadangID == 0)
                    {
                        dbSP.SetCommandTimeout(120);
                        resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahID, LadangID, Status, UserID).ToList();
                    }
                    else
                    {
                        dbSP.SetCommandTimeout(120);
                        resultreport = dbSP.sp_RptRumKedKepPekLad(NegaraID, SyarikatID, WilayahID, LadangID, Status, UserID).ToList();
                    }
                }

                WorkerActiveReportViewer.LocalReport.ReportPath = Server.MapPath("~/Reports/RDLC/ReportActiveWorker.rdlc");
                WorkerActiveReportViewer.LocalReport.DataSources.Clear();
                ReportDataSource rdc = new ReportDataSource("ActiveWorkerDataSet", resultreport);
                WorkerActiveReportViewer.LocalReport.DataSources.Add(rdc);
                WorkerActiveReportViewer.LocalReport.Refresh();
                WorkerActiveReportViewer.DataBind();
            }
        }
    }
}