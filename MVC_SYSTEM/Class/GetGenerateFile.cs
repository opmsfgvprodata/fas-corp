using MVC_SYSTEM.Models; //fatin added - 19/11/2023
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetGenerateFile
    {
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private GetTriager GetTriager = new GetTriager();
        private static GetNSWL GetNSWL = new GetNSWL(); //fatin added - 19/11/2023

        public string GenFileMaybank(List<ModelsSP.sp_SocsoFileGen_Result> SoscoDetails, string bulan, string tahun, out string filename)
        {
            string filePath = "~/SocsoFile/" + tahun + "/" + bulan + "/";
            string path = HttpContext.Current.Server.MapPath(filePath);
            filename = "SOCSO-" + timezone.gettimezone().ToString("ddMMyyyy") + ".txt";
            string filecreation = path + filename;
            int rowno = 1;
            DateTime NowDate = timezone.gettimezone();
            TryToDelete(filecreation);
            if (!Directory.Exists(path))
            {
                //If No any such directory then creates the new one
                Directory.CreateDirectory(path);
            }
            decimal? Amount = 0;
            decimal AmountC = 0;
            int AmountInt = 0;
            string Body1 = "";
            string Body2 = "";
            string Body3 = "";
            string Body4 = "";
            string Body5 = "";
            string Body6 = "";

            using (StreamWriter writer = new StreamWriter(filecreation, true))
            {
                foreach (var SoscoDetail in SoscoDetails)
                {
                    Body1 = SoscoDetail.fld_EmployeeCode;
                    Body2 = SoscoDetail.fld_ICNo.Replace("-","");
                    Body3 = SoscoDetail.fld_Name.Trim();
                    Body4 = bulan + tahun;
                    Amount = SoscoDetail.fld_Amount * 100;
                    AmountC = Math.Round((decimal)Amount, 0);
                    AmountInt = int.Parse(AmountC.ToString());
                    Body5 = AmountInt.ToString();
                    

                    writer.Write(Body1.PadRight(32, ' ')); //
                    writer.Write(Body2.PadRight(12, ' ')); //
                    writer.Write(Body3.PadRight(150, ' ')); //
                    writer.Write(Body4.PadRight(6, ' ')); //
                    writer.Write(Body5.PadLeft(14, '0')); //
                    writer.WriteLine(Body6.PadRight(9, ' ')); //
                    
                    rowno++;
                }
                rowno = rowno - 1;
            }

            return filePath;
        }
        //fatin added - 19/11/2023
        public static string GenerateFileMaybank(List<ModelsSP.sp_MaybankRcms_Result> maybankrcmsList, tbl_Syarikat tbl_Syarikat, string bulan, string tahun, int? NegaraID, int? SyarikatID, /*int? WilayahID,*/ string CompCode, string filter, DateTime PaymentDate, out string filename)
        {
            decimal? TotalGaji = 0;
            int CountData = 0;
            int rowno = 1;
            int SalaryInt = 0;
            int SalaryHash = 0;
            int SixDigitAccNoInt = 0;
            int reminder = 0;
            int AccountHash = 0;
            int TotalHash = 0;
            int SumAllTotalHash = 0;
            int onedigit = 0;
            string statusmsg = "";
            string PaymentCode = "";
            string ResidentIInd = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            string ClientIDText = "";
            char onechar;
            //DateTime? date = timezone.gettimezone();
            //DateTime Today = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            //DateTime? PaymentDate = new DateTime(Today.Year, Today.Month, 5);
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(CompCode, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" /*+ WilayahID.ToString() + "/"*/;
            string path = HttpContext.Current.Server.MapPath(filePath);

            filename = "M2E LABOR (" + tbl_Syarikat.fld_NamaPndkSyarikat.ToUpper() + ") " +  " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                    CountData = maybankrcmsList.Count();
                }               

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 28;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else if (i == 2)
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }

                            //if (ClientID == null || ClientID == " ")
                            //{
                            //    if (CompCode == "FASSB")
                            //    {
                            //        Header.Insert(i, "FGVASB" + bulan + tahun + "|");
                            //    }

                            //    if (CompCode == "RNDSB")
                            //    {
                            //        Header.Insert(i, "RNDSB" + bulan + tahun + "|");
                            //    }
                            //}
                            //else
                            //{
                            //    Header.Insert(i, ClientID + "|");
                            //}
                        }                       
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";
                        string WorkerName2 = "";
                        string WorkerName3 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 40)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }
                        if (WorkerNameLength > 40 && WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, WorkerNameLength - 40);
                        }
                        if (WorkerNameLength > 80 && WorkerNameLength <= 120)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, 40);
                            WorkerName3 = maybankrcms.fld_Nama.Substring(80, WorkerNameLength - 80);
                        }

                        //***SalaryHashing***
                        SalaryInt = (int)(maybankrcms.fld_GajiBersih * 100);
                        SalaryHash = (SalaryInt % 2000) + rowno;

                        //**AccountHashing***
                        AccountHash = 0;
                        AccNoWorker = maybankrcms.fld_NoAkaun;
                        if (AccNoWorker == "" || AccNoWorker == null) //space (ASCII) = 32
                        {
                            AccountHash = ((32 * 6) * 2) + rowno;
                        }
                        else if (AccNoWorker == "0")
                        {
                            AccountHash = ((0 * 6) * 2) + rowno;
                        }
                        else
                        {
                            if (AccNoWorker.Length < 6)
                            {
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                            else
                            {
                                AccNoWorker = AccNoWorker.Substring(AccNoWorker.Length - 6, 6);
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }

                                //SixDigitAccNoInt = int.Parse(maybankrcms.fld_NoAkaun.Substring(maybankrcms.fld_NoAkaun.Length - 6, 6));
                                //while (SixDigitAccNoInt > 0)
                                //{
                                //    reminder = SixDigitAccNoInt % 10;
                                //    AccountHash = AccountHash + reminder;
                                //    SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                //}
                                //AccountHash = (AccountHash * 2) + rowno;
                            }
                        }

                        //**TotalHash***
                        TotalHash = SalaryHash + AccountHash;
                        SumAllTotalHash = SumAllTotalHash + TotalHash;

                        //start write body
                        int BodyLoop = 337;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 1) //2
                            {
                                PaymentCode = maybankrcms.fld_RcmsBankCode == "MBBEMYKL" ? "IT" : "IG";
                                Body.Insert(i, PaymentCode + "|");
                            }
                            else if (i == 2) //3
                            {
                                Body.Insert(i, "Staff Payroll|");
                            }
                            else if (i == 4) //5
                            {
                                Body.Insert(i, string.Format("{0:ddMMyyyy}", PaymentDateFormat) + "|");
                            }
                            else if (i == 7) //8
                            {
                                //Body.Insert(i, maybankrcms.fld_Nokp.ToUpper() + "|");
                                Body.Insert(i, maybankrcms.fld_Nopkj.ToUpper() + "|");
                            }
                            else if (i == 8) //9
                            {
                                Body.Insert(i, "SALARY " + bulan + tahun + "|");
                                //Body.Insert(i, "MAPA " + bulan + tahun.Substring(2, 2) + "|");
                            }
                            else if (i == 9) //10
                            {
                                //Body.Insert(i, maybankrcms.fld_LdgName + "|");
                                Body.Insert(i, maybankrcms.fld_LdgShortName + "|");                               
                            }
                            else if (i == 10) //11
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 11) //12
                            {
                                Body.Insert(i, maybankrcms.fld_GajiBersih + "|");
                            }
                            else if (i == 12) //13
                            {
                                Body.Insert(i, "Y|");
                            }
                            else if (i == 13) //14
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 14) //15
                            {
                                Body.Insert(i, AccNo + "|");
                            }
                            else if (i == 15) //16
                            {
                                Body.Insert(i, maybankrcms.fld_NoAkaun + "|");
                            }
                            else if (i == 18) //19
                            {
                                ResidentIInd = maybankrcms.fld_Kdrkyt == "MA" ? "Y" : "N";
                                Body.Insert(i, ResidentIInd + "|");
                            }
                            else if (i == 19) //20
                            {
                                if (WorkerName1 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName1 + "|");
                                }
                            }
                            else if (i == 20) //21
                            {
                                if (WorkerName2 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName2 + "|");
                                }
                            }
                            else if (i == 21) //22
                            {
                                if (WorkerName3 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName3 + "|");
                                }
                            }
                            else if (i == 23) //24
                            {
                               Body.Insert(i, maybankrcms.fld_Nopkj + "|");                               
                            }
                            else if (i == 24) //25
                            {
                               Body.Insert(i, maybankrcms.fld_Nokp + "|");
                            }
                            //else if (i == 36) //37
                            //{
                            //    Body.Insert(i, maybankrcms.fld_RcmsBankCode + "|");
                            //}
                            //else if (i == 102) //103
                            //{
                            //    if (CompCode == "FASSB")
                            //    {
                            //        Body.Insert(i, "FASSB " + /*tbl_Wilayah.fld_WlyhName.ToUpper() +*/ "|");
                            //    }
                            //    else if (CompCode == "RNDSB")
                            //    {
                            //        Body.Insert(i, "RNDSB " + /*tbl_Wilayah.fld_WlyhName.ToUpper() +*/ "|");
                            //    }
                            //}
                            else if (i == 109) //110
                            {
                                Body.Insert(i, "01|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, CountData + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalGaji + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

        public static string GenerateFileMaybankOthers(List<ModelsSP.sp_MaybankRcmsOthers_Result> maybankrcmsList, tbl_Syarikat tbl_Syarikat, string bulan, string tahun, int? NegaraID, int? SyarikatID, string CompCode, string filter, DateTime PaymentDate, string Incentive, out string filename)
        {
            decimal? TotalGaji = 0;
            int CountData = 0;
            int rowno = 1;
            int SalaryInt = 0;
            int SalaryHash = 0;
            int SixDigitAccNoInt = 0;
            int reminder = 0;
            int AccountHash = 0;
            int TotalHash = 0;
            int SumAllTotalHash = 0;
            int onedigit = 0;
            string statusmsg = "";
            string PaymentCode = "";
            string ResidentIInd = "";
            string CorpID = "";
            string ClientID = "";
            string AccNo = "";
            string InitialName = "";
            string AccNoWorker = "";
            string ClientIDText = "";
            char onechar;
            //DateTime? date = timezone.gettimezone();
            //DateTime Today = new DateTime(date.Value.Year, date.Value.Month, date.Value.Day);
            //DateTime? PaymentDate = new DateTime(Today.Year, Today.Month, 5);
            DateTime? PaymentDateFormat = new DateTime(PaymentDate.Year, PaymentDate.Month, PaymentDate.Day);

            GetNSWL.GetSyarikatRCMSDetail(CompCode, out CorpID, out ClientID, out AccNo, out InitialName);
            string filePath = "~/MaybankFile/" + tahun + "/" + bulan + "/" + NegaraID.ToString() + "_" + SyarikatID.ToString() + "/" /*+ WilayahID.ToString() + "/"*/;
            string path = HttpContext.Current.Server.MapPath(filePath);

            filename = "M2E LABOR (" + tbl_Syarikat.fld_NamaPndkSyarikat.ToUpper() + ") " + " " + bulan + tahun + ".txt";
            string filecreation = path + filename;

            try
            {
                TryToDelete(filecreation);
                if (!Directory.Exists(path))
                {
                    //If No any such directory then creates the new one
                    Directory.CreateDirectory(path);
                }

                if (maybankrcmsList.Count() != 0)
                {
                    TotalGaji = maybankrcmsList.Sum(s => s.fld_GajiBersih);
                    CountData = maybankrcmsList.Count();
                }

                using (StreamWriter writer = new StreamWriter(filecreation, true))
                {
                    //header
                    int HeaderLoop = 28;
                    ArrayList Header = new ArrayList();
                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == 0)
                        {
                            Header.Insert(i, "00|");
                        }
                        else if (i == 1)
                        {
                            Header.Insert(i, CorpID + "|");
                        }
                        else if (i == 2)
                        {
                            if (filter == "" || filter == null)
                            {
                                Header.Insert(i, ClientID + "|");
                            }
                            else
                            {
                                Header.Insert(i, filter + "|");
                            }

                            //if (ClientID == null || ClientID == " ")
                            //{
                            //    if (CompCode == "FASSB")
                            //    {
                            //        Header.Insert(i, "FGVASB" + bulan + tahun + "|");
                            //    }

                            //    if (CompCode == "RNDSB")
                            //    {
                            //        Header.Insert(i, "RNDSB" + bulan + tahun + "|");
                            //    }
                            //}
                            //else
                            //{
                            //    Header.Insert(i, ClientID + "|");
                            //}
                        }
                        else
                        {
                            Header.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= HeaderLoop; i++)
                    {
                        if (i == HeaderLoop)
                        {
                            writer.WriteLine(Header[i]);
                        }
                        else
                        {
                            writer.Write(Header[i]);
                        }
                    }

                    //body                
                    foreach (var maybankrcms in maybankrcmsList)
                    {
                        int WorkerNameLength = 0;
                        string WorkerName1 = "";
                        string WorkerName2 = "";
                        string WorkerName3 = "";

                        WorkerNameLength = maybankrcms.fld_Nama.Length;
                        if (WorkerNameLength <= 40)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, WorkerNameLength);
                        }
                        if (WorkerNameLength > 40 && WorkerNameLength <= 80)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, WorkerNameLength - 40);
                        }
                        if (WorkerNameLength > 80 && WorkerNameLength <= 120)
                        {
                            WorkerName1 = maybankrcms.fld_Nama.Substring(0, 40);
                            WorkerName2 = maybankrcms.fld_Nama.Substring(40, 40);
                            WorkerName3 = maybankrcms.fld_Nama.Substring(80, WorkerNameLength - 80);
                        }

                        //***SalaryHashing***
                        SalaryInt = (int)(maybankrcms.fld_GajiBersih * 100);
                        SalaryHash = (SalaryInt % 2000) + rowno;

                        //**AccountHashing***
                        AccountHash = 0;
                        AccNoWorker = maybankrcms.fld_NoAkaun;
                        if (AccNoWorker == "" || AccNoWorker == null) //space (ASCII) = 32
                        {
                            AccountHash = ((32 * 6) * 2) + rowno;
                        }
                        else if (AccNoWorker == "0")
                        {
                            AccountHash = ((0 * 6) * 2) + rowno;
                        }
                        else
                        {
                            if (AccNoWorker.Length < 6)
                            {
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                            }
                            else
                            {
                                AccNoWorker = AccNoWorker.Substring(AccNoWorker.Length - 6, 6);
                                var isValid = AccNoWorker.All(c => char.IsDigit(c)); //check whole number is numeric or not
                                if (isValid)
                                {
                                    SixDigitAccNoInt = int.Parse(AccNoWorker);
                                    while (SixDigitAccNoInt > 0)
                                    {
                                        reminder = SixDigitAccNoInt % 10;
                                        AccountHash = AccountHash + reminder;
                                        SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }
                                else
                                {
                                    for (int i = 0; i < AccNoWorker.Length; i++)
                                    {
                                        onechar = AccNoWorker.ElementAt(i);
                                        var isValidC = char.IsLetter(onechar); // to checj each char is numeric or not
                                        if (isValidC)
                                        {
                                            onedigit = (int)Char.GetNumericValue(onechar);
                                        }
                                        else
                                        {
                                            onedigit = Int32.Parse(onechar.ToString());
                                        }

                                        AccountHash = AccountHash + onedigit;
                                    }
                                    AccountHash = (AccountHash * 2) + rowno;
                                }

                                //SixDigitAccNoInt = int.Parse(maybankrcms.fld_NoAkaun.Substring(maybankrcms.fld_NoAkaun.Length - 6, 6));
                                //while (SixDigitAccNoInt > 0)
                                //{
                                //    reminder = SixDigitAccNoInt % 10;
                                //    AccountHash = AccountHash + reminder;
                                //    SixDigitAccNoInt = SixDigitAccNoInt / 10;
                                //}
                                //AccountHash = (AccountHash * 2) + rowno;
                            }
                        }

                        //**TotalHash***
                        TotalHash = SalaryHash + AccountHash;
                        SumAllTotalHash = SumAllTotalHash + TotalHash;

                        //start write body
                        int BodyLoop = 337;
                        ArrayList Body = new ArrayList();
                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == 0) //1
                            {
                                Body.Insert(i, "01|");
                            }
                            else if (i == 1) //2
                            {
                                PaymentCode = maybankrcms.fld_RcmsBankCode == "MBBEMYKL" ? "IT" : "IG";
                                Body.Insert(i, PaymentCode + "|");
                            }
                            else if (i == 2) //3
                            {
                                Body.Insert(i, "Staff Payroll|");
                            }
                            else if (i == 4) //5
                            {
                                Body.Insert(i, string.Format("{0:ddMMyyyy}", PaymentDateFormat) + "|");
                            }
                            else if (i == 7) //8
                            {
                                //Body.Insert(i, maybankrcms.fld_Nokp.ToUpper() + "|");
                                Body.Insert(i, maybankrcms.fld_Nopkj.ToUpper() + "|");
                            }
                            else if (i == 8) //9
                            {
                                Body.Insert(i, "SALARY " + bulan + tahun + "|");
                                //Body.Insert(i, "MAPA " + bulan + tahun.Substring(2, 2) + "|");
                            }
                            else if (i == 9) //10
                            {
                                //Body.Insert(i, maybankrcms.fld_LdgName + "|");
                                Body.Insert(i, maybankrcms.fld_LdgShortName + "|");
                            }
                            else if (i == 10) //11
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 11) //12
                            {
                                Body.Insert(i, maybankrcms.fld_GajiBersih + "|");
                            }
                            else if (i == 12) //13
                            {
                                Body.Insert(i, "Y|");
                            }
                            else if (i == 13) //14
                            {
                                Body.Insert(i, "MYR|");
                            }
                            else if (i == 14) //15
                            {
                                Body.Insert(i, AccNo + "|");
                            }
                            else if (i == 15) //16
                            {
                                Body.Insert(i, maybankrcms.fld_NoAkaun + "|");
                            }
                            else if (i == 18) //19
                            {
                                ResidentIInd = maybankrcms.fld_Kdrkyt == "MA" ? "Y" : "N";
                                Body.Insert(i, ResidentIInd + "|");
                            }
                            else if (i == 19) //20
                            {
                                if (WorkerName1 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName1 + "|");
                                }
                            }
                            else if (i == 20) //21
                            {
                                if (WorkerName2 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName2 + "|");
                                }
                            }
                            else if (i == 21) //22
                            {
                                if (WorkerName3 == "")
                                {
                                    Body.Insert(i, "|");
                                }
                                else
                                {
                                    Body.Insert(i, WorkerName3 + "|");
                                }
                            }
                            else if (i == 23) //24
                            {
                                Body.Insert(i, maybankrcms.fld_Nopkj + "|");
                            }
                            else if (i == 24) //25
                            {
                                Body.Insert(i, maybankrcms.fld_Nokp + "|");
                            }
                            //else if (i == 36) //37
                            //{
                            //    Body.Insert(i, maybankrcms.fld_RcmsBankCode + "|");
                            //}
                            //else if (i == 102) //103
                            //{
                            //    if (CompCode == "FASSB")
                            //    {
                            //        Body.Insert(i, "FASSB " + /*tbl_Wilayah.fld_WlyhName.ToUpper() +*/ "|");
                            //    }
                            //    else if (CompCode == "RNDSB")
                            //    {
                            //        Body.Insert(i, "RNDSB " + /*tbl_Wilayah.fld_WlyhName.ToUpper() +*/ "|");
                            //    }
                            //}
                            else if (i == 109) //110
                            {
                                Body.Insert(i, "01|");
                            }
                            else
                            {
                                Body.Insert(i, "|");
                            }
                        }

                        for (int i = 0; i <= BodyLoop; i++)
                        {
                            if (i == BodyLoop)
                            {
                                writer.WriteLine(Body[i]);
                            }
                            else
                            {
                                writer.Write(Body[i]);
                            }
                        }
                        rowno++;
                    }//close foreach

                    //footer
                    int FooterLoop = 28;
                    ArrayList Footer = new ArrayList();
                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == 0)//1
                        {
                            Footer.Insert(i, "99|");
                        }
                        else if (i == 1)//2
                        {
                            Footer.Insert(i, CountData + "|");
                        }
                        else if (i == 2)//3
                        {
                            Footer.Insert(i, TotalGaji + "|");
                        }
                        else if (i == 3)//4
                        {
                            Footer.Insert(i, SumAllTotalHash + "|");
                        }
                        else
                        {
                            Footer.Insert(i, "|");
                        }
                    }

                    for (int i = 0; i <= FooterLoop; i++)
                    {
                        if (i == FooterLoop)
                        {
                            writer.WriteLine(Footer[i]);
                        }
                        else
                        {
                            writer.Write(Footer[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                //msg = GlobalResCorp.msgGenerateFailed;
                statusmsg = ex.Message;
            }
            return filePath;
        }

        public string CreateTextFile(string filename, string fileContent, string subFolderName)
        {
            string folderPath = "~/TextFile/" + subFolderName + "/";
            string path = HttpContext.Current.Server.MapPath(folderPath);
            string filecreation = path + filename;

            TryToDelete(filecreation);

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (StreamWriter writer = new StreamWriter(filecreation, true))
            {
                writer.WriteLine(fileContent);
                writer.Close();
            }

            return folderPath;
        }

        static bool TryToDelete(string f)
        {
            try
            {
                // A.
                // Try to delete the file.
                File.Delete(f);
                return true;
            }
            catch (IOException)
            {
                // B.
                // We could not delete the file.
                return false;
            }
        }
    }
}