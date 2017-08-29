using Foodbook.DataAccess;
using Foodbook.WebApi.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Foodbook.WebApi.Controllers
{

    public class CookCommentController : BaseController
    {
        [Authorize]
        public IHttpActionResult Post([FromBody] PostCookCommentModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    string aspUserId = User.Identity.GetUserId();

                    Cook cook = DbContext.Cooks.FirstOrDefault(x => x.ApsUserId.Equals(aspUserId));

                    CookComment comment = new CookComment
                    {
                        CommentText = model.CommentText,
                        CommentOwnerId = cook.CookId,
                        DateInserted = model.InsertDate,
                        Rating = model.Rating,
                        CommentedCookId = model.CookId
                    };

                    DbContext.CookComments.Add(comment);
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
