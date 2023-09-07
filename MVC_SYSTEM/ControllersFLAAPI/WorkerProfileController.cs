using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MVC_SYSTEM.ModelsSP;

namespace MVC_SYSTEM.ControllersFLAAPI
{
    public class WorkerProfileController : ApiController
    {
        private MVC_SYSTEM_SP2_Models dbSP = new MVC_SYSTEM_SP2_Models();

        // GET api/<controller>
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<controller>/5
        public List<sp_FLAWorkerProfile_Result> Get(int Month, int Year)
        {
            List<sp_FLAWorkerProfile_Result> flaWorkerProfile = new List<sp_FLAWorkerProfile_Result>();
            try
            {
                flaWorkerProfile = dbSP.sp_FLAWorkerProfile(Month, Year).ToList();
            }
            catch (Exception exception)
            {
                //geterror.catcherro(exception.Message, exception.StackTrace, exception.Source, exception.TargetSite.ToString());
            }
            return flaWorkerProfile;

        }

        // POST api/<controller>
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}