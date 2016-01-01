using EFunTech.Sms.Portal.Controllers.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using EFunTech.Sms.Schema;
using EntityFramework.Caching;
using EntityFramework.Extensions;

namespace EFunTech.Sms.Portal.Controllers
{
    public class TestController : ApiControllerBase
    {
        public TestController(DbContext context, ILogService logService)
            : base(context, logService)
        {
        }

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            //return await context.Set<ApplicationUser>()
            //    .Select(p => p.UserName)
            //    .ToListAsync();

            return context.Set<ApplicationUser>()
                .Select(p => p.UserName)
                .ToList();
        }


        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}