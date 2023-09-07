using System;
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