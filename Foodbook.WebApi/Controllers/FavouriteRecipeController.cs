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
    public class FavouriteRecipeController : BaseController
    {
        [Authorize]
        public IHttpActionResult Put(long id)
        {
            try
            {
                string aspUserId = User.Identity.GetUserId();
                Cook cook = DbContext.Cooks.FirstOrDefault(x => x.ApsUserId.Equals(aspUserId));
                Recipe recipe = DbContext.Recipes.Find(id);
                if (cook != null && recipe != null)
                {
                    if (cook.Recipes1.Any(x => x.RecipeId == id))
                    {
                        cook.Recipes1.Remove(recipe);
                    }
                    else
                    {
                        cook.Recipes1.Add(recipe);
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
