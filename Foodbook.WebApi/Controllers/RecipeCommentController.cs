using Foodbook.DataAccess;
using Foodbook.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Foodbook.WebApi.Controllers
{

    public class RecipeCommentController : BaseController
    {
        public IHttpActionResult Post([FromBody] PostRecipeCommentModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    RecipeComment comment = new RecipeComment
                    {
                        CommentText = model.CommentText,
                        CookId = model.CookId,
                        DateInserted = model.InsertDate,
                        Rating = model.Rating,
                        RecipeId = model.RecipeId
                    };

                    DbContext.RecipeComments.Add(comment);
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
