using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Foodbook.WebApi.Controllers
{
    public class LogController : ApiController
    {


        // POST api/<controller>
        public void Post([FromBody]string value)
        {
            int i = 2;
        }

    }
}