using Foodbook.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Foodbook.WebApi.Controllers
{
    public abstract class BaseController : ApiController
    {
        protected FoodbookEntities DbContext;

        public BaseController()
        {
            DbContext = new FoodbookEntities();
        }

        protected override void Dispose(bool disposing)
        {
            DbContext.Dispose();
            base.Dispose(disposing);
        }
    }
}