using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ViewingModels;
using MVC_SYSTEM.App_LocalResources;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ModelsSP;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User,Normal User")]
    public class MaybankFileGenPdfController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate dbC = new MVC_SYSTEM_ModelsCorporate();
        // private MVC_SYSTEM_MasterModels db = new MVC_SYSTEM_MasterModels();
        GetIdentity GetIdentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetTriager GetTriager = new GetTriager();
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();
        // GET: ReportsPdf
        public FileStreamResult MaybankFileGenPdf(string CompCodeList, int? MonthList, int? YearList, string print, string PaymentDate)
        {
            int? NegaraID, SyarikatID, WilayahID, LadangID;
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            //Connection.GetConnection(out host, out catalog, out user, out pass, WilayahID.Value, SyarikatID.Value, NegaraID.Value);
            // MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);
            List<sp_MaybankRcmsZAP64_Result> maybankrcmsZAP64 = new List<sp_MaybankRcmsZAP64_Result>();


            string NamaSyarikat = dbC.tbl_Syarikat.Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
            string NamaPendekSyarikat = dbC.tbl_Syarikat
               .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
               .Select(s => s.fld_NamaPndkSyarikat)
               .FirstOrDefault();
            string NoSyarikat = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_NoSyarikat)
                .FirstOrDefault();
            String CorpID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_CorporateID.ToString())
                .FirstOrDefault();
            String ClientID = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_ClientBatchID.ToString())
                .FirstOrDefault();
            string AccNo = dbC.tbl_Syarikat
                .Where(x => x.fld_NegaraID == NegaraID && x.fld_NamaPndkSyarikat == CompCodeList)
                .Select(s => s.fld_AccountNo.ToString())
                .FirstOrDefault();

            ViewBag.NegaraID = NegaraID;
            ViewBag.SyarikatID = SyarikatID;
            ViewBag.UserID = getuserid;
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Date = DateTime.Now.ToShortDateString();
            ViewBag.Time = DateTime.Now.ToShortTimeString();

            if (YearList == null && MonthList == null)
            {
                ViewBag.DocDate = DateTime.Now.AddMonths(+1).AddDays(-DateTime.Now.Day).ToString("dd.MM.yyyy");
            }
            else
            {
                var lastday = DateTime.DaysInMonth(YearList.Value, MonthList.Value);
                ViewBag.DocDate = lastday + "." + MonthList + "." + YearList;
            }
            string PostingDate = Convert.ToDateTime(PaymentDate).ToString("dd.MM.yyyy");
            ViewBag.Description = "Region " + NamaSyarikat + " - Maybank Rcms ZAP64 for " + MonthList + "/" + YearList;
            if (MonthList == null || YearList == null || CompCodeList == "0")
            {
                ViewBag.Message = "Please select month, year, company and payment date";

            }

            dbSP.SetCommandTimeout(2400);
            maybankrcmsZAP64 = dbSP.sp_MaybankRcmsZAP64(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).ToList();

            var BankList = dbC.tbl_Bank
                .Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false)
                .ToList();

            ViewBag.RecordNo = maybankrcmsZAP64.Count();

            if (maybankrcmsZAP64.Count() == 0)
            {
                ViewBag.Message = GlobalResCorp.msgNoRecord;
            }

            dbSP.SetCommandTimeout(2400);
            var maybankDetails = dbSP.sp_MaybankRcmsOnlinePaymentRpt(NegaraID, SyarikatID, YearList, MonthList, getuserid, CompCodeList).ToList();
            decimal? TotalGajiBersih = 0;
            int? TotalPekerja = 0;
            if (maybankDetails != null)
            {
                TotalGajiBersih = maybankDetails.Select(x => x.fld_JumlahGajiBersih).Sum();
                TotalPekerja = maybankDetails.Select(x => x.fld_JumlahPekerja).Sum();
            }

            Document pdfDoc = new Document(PageSize.A4, 40, 40, 30, 20);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();
            //var pkjList = new List<Models.tbl_Pkjmast>();

            if (maybankDetails.Count() > 0)
            {

                pdfDoc.NewPage();
                //Header
                pdfDoc = Header(pdfDoc, NamaSyarikat, "(" + NoSyarikat + ")\n", "Laporan Slip Gaji Pekerja Bagi Bulan " + MonthList + "/" + YearList + "");
                ////Header
                PdfPTable table = new PdfPTable(2);
                table.WidthPercentage = 100;
                table.SpacingBefore = 10f;
                float[] widths = new float[] { 1.5f, 0.5f };
                table.SetWidths(widths);
                PdfPCell cell = new PdfPCell();

                chunk = new Chunk("M2E Payroll Payment Instruction Letter", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                cell.Border = Rectangle.TOP_BORDER;
                cell.BorderColor = BaseColor.ORANGE;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk("Date: " + DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk("The Manager", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk("MALAYAN BANKING BERHAD", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk("14217 - MAYBANK JALAN MAKTAB", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk("Dear Sir ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk("Kindly please approve payroll webcycle for salary payment as follow :", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);



                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                cell = new PdfPCell();
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                pdfDoc.Add(table);

                table = new PdfPTable(2);
                table.WidthPercentage = 48;
                table.SpacingBefore = 5f;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                widths = new float[] { 1, 1 };
                table.SetWidths(widths);

                chunk = new Chunk("Payment Reference", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : Monthly Salary Crediting", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk("Payment Description ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : SALARY " + MonthList + "/" + YearList, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk("Client Code", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : " + CorpID, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk("Originator Name", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : " + NamaSyarikat, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk("Originator Account For Debit", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : " + AccNo, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                pdfDoc.Add(table);

                /////////////////
                table = new PdfPTable(3);
                table.WidthPercentage = 60;
                table.SpacingBefore = 5f;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                widths = new float[] { 1, 1, 1 };
                table.SetWidths(widths);

                chunk = new Chunk("Salary Crediting", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Colspan = 3;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("Amount(RM)", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("Head Count", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("   (A)Amount to be Credited", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(TotalPekerja.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("     a. Total Bank", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(TotalGajiBersih.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(TotalPekerja.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("     b. Total Cheque", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("     c. Total Cash", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk("0.00", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);


                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER | Rectangle.LEFT_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                pdfDoc.Add(table);

                ////////////////////
                table = new PdfPTable(2);
                table.WidthPercentage = 50;
                table.SpacingBefore = 5f;
                table.HorizontalAlignment = Element.ALIGN_LEFT;
                widths = new float[] { 1, 1 };
                table.SetWidths(widths);

                chunk = new Chunk("     Crediting Date", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : " + PostingDate, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk("     Highest to be credited", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk("     Lowest to be credited", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" : ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);


                chunk = new Chunk("Your cooperation is highly appreciated", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);

                chunk = new Chunk("Thank you", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                cell.Colspan = 2;
                table.AddCell(cell);
                pdfDoc.Add(table);
                ////////////////////
                table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SpacingBefore = 5f;
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                widths = new float[] { 1, 1, 1, 1 };
                table.SetWidths(widths);

                chunk = new Chunk("Prepared by,", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk("Approved by,", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = Rectangle.BOTTOM_BORDER;
                cell.BorderColor = BaseColor.BLACK;
                table.AddCell(cell);

                chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);

            }

            else
            {
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }
            //pdfDoc = Footer(pdfDoc, chunk, para);
            //pdfDoc = Footer(pdfDoc, chunk, para);
            //pdfDoc = Footer(pdfDoc, chunk, para);
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");




            //public Document WorkerPaySlipContent(Document pdfDoc, List<sp_Payslip_Result> item)
            //{
            //    return pdfDoc;
            //}
        }
        public Document Header(Document pdfDoc, string headername, string headername2, string headername3)
        {
            //Paragraph date = new Paragraph(new Chunk("Tarikh : " + timezone.gettimezone().ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
            //date.Alignment = Element.ALIGN_RIGHT;
            //pdfDoc.Add(date);
            PdfPTable table = new PdfPTable(2);
            table.WidthPercentage = 100;
            float[] widths = new float[] { 1, 1 };
            table.SetWidths(widths);

            PdfPCell cell = new PdfPCell();
            Chunk chunk = new Chunk(headername, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);

            Image image = Image.GetInstance(Server.MapPath("~/Asset/Images/logo2.jpg"));
            PdfPCell cell1 = new PdfPCell(image);
            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
            cell1.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell1.Border = 0;
            image.ScaleAbsolute(60, 60);
            table.AddCell(cell1);




            //chunk = new Chunk(headername2, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            //cell = new PdfPCell(new Phrase(chunk));
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell.Border = 0;
            //table.AddCell(cell);
            //chunk = new Chunk(headername3, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            //cell = new PdfPCell(new Phrase(chunk));
            //cell.HorizontalAlignment = Element.ALIGN_CENTER;
            //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            //cell.Border = 0;
            //table.AddCell(cell);
            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document Footer(Document pdfDoc, Chunk chunk, Paragraph para)
        {

            return pdfDoc;
        }

    }
}