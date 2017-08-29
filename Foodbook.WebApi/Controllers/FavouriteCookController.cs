using Foodbook.DataAccess;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Foodbook.WebApi.Controllers
{
    public class FavouriteCookController : BaseController
    {
        [Authorize]
        public IHttpActionResult Put(long id)
        {
            try
            {
                string aspUserId = User.Identity.GetUserId();
                Cook cook = DbContext.Cooks.FirstOrDefault(x => x.ApsUserId.Equals(aspUserId));
                Cook followedCook = DbContext.Cooks.Find(id);
                if (cook != null && followedCook != null)
                {
                    if (cook.Cooks.Any(x => x.CookId == id))
                    {
                        cook.Cooks.Remove(followedCook);
                    }
                    else
                    {
                        cook.Cooks.Add(followedCook);
                    }

                    DbContext.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }
    }
}
