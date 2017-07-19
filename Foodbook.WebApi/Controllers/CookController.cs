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
    public class CookController : BaseController
    {
        public IHttpActionResult Get(long id)
        {
            try
            {
                Cook cook = DbContext.Cooks.Find(id);

                if (cook == null)
                {
                    return BadRequest();
                }
                else
                {
                    ResponseCookModel model = new ResponseCookModel
                    {
                        CookId = cook.CookId,
                        Biography = cook.Bio,
                        FirstName = cook.FirstName,
                        LastName = cook.LastName,
                        PhotoUrl = cook.PhotoUrl,
                        Recipes = cook.Recipes.ToList().Select(x => InitRecipeModel(x)).ToList(),
                        FavouriteRecipes = cook.Recipes1.ToList().Select(x => InitRecipeModel(x)).ToList(),
                        FollowedCooks = cook.Cooks.Select(x => new CookModel
                        {
                            CookId = x.CookId,
                            Biography = x.Bio,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            PhotoUrl = x.PhotoUrl
                        }).ToList()                           
                    };

                    return Ok(model);
                }                
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Put(long id, [FromBody] CookModel model)
        {
            try
            {
                Cook cook = DbContext.Cooks.Find(id);

                if (ModelState.IsValid && cook != null)
                {
                    cook.FirstName = model.FirstName;
                    cook.LastName = model.LastName;
                    cook.PhotoUrl = model.PhotoUrl;
                    cook.Bio = model.Biography;

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

        private RecipeModel InitRecipeModel(Recipe x)
        {
            return new RecipeModel
            {
                RecipeId = x.RecipeId,
                CookId = x.CookId,
                Name = x.Name,
                CaloricityId = x.CaloricityId,
                CaloricityName = x.Caloricity?.Name,
                CategoryId = x.FoodCategoryId,
                CategoryName = x.FoodCategory.CategoryName,
                CuisineId = x.CuisineId,
                CausineName = x.Cuisine.CuisineName,
                CookName = string.Join(" ", x.Cook.FirstName, x.Cook.LastName),
                VideoUrl = x.VideoUrl,
                InsertDate = x.InsertDate,
                Rating = x.RecipeComments.Any() ? (double?)x.RecipeComments.Sum(z => z.Rating) / x.RecipeComments.Count() : null,
                RecipeText = x.RecipeText,
                Comments = x.RecipeComments.ToList().Select(z => new CommentModel
                {
                    CommentId = z.CommentId,
                    CookId = z.CookId,
                    CookName = string.Join(" ", z.Cook.FirstName, z.Cook.LastName),
                    CommentText = z.CommentText,
                    InsertDate = z.DateInserted,
                    Rating = z.Rating

                }).ToList()
            };
        }


    }
}
