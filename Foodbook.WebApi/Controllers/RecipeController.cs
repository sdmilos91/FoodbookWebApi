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
    public class RecipeController : BaseController
    {
        public IHttpActionResult Get(long id)
        {
            try
            {
                Recipe recipe = DbContext.Recipes.Find(id);

                if (recipe != null)
                {
                    RecipeModel model = InitRecipeModel(recipe);

                    return Ok(model);
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

        public IHttpActionResult Get(string text = "", long? categoryId = null, long? cousineId = null)
        {
            try
            {
                List<Recipe> recipes = DbContext.Recipes.Where(x => (string.IsNullOrEmpty(text) ? true : x.Name.ToLower().Contains(text.ToLower()))
                                                                 && (categoryId.HasValue ? x.FoodCategoryId == categoryId.Value : true)
                                                                 && (cousineId.HasValue ? x.CuisineId == cousineId.Value : true)
                                                                ).ToList();

                List<RecipeModel> models = recipes.Select(x => InitRecipeModel(x)).ToList();

                return Ok(models);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);                
            }
        }

        [Authorize]
        public IHttpActionResult Post([FromBody] PostRecipeModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string aspUserId = User.Identity.GetUserId();

                    Cook cook = DbContext.Cooks.FirstOrDefault(x => x.ApsUserId.Equals(aspUserId));

                    Recipe recipe = new Recipe
                    {
                        Name = model.Name,
                        RecipeText = model.RecipeText,
                        CookId = cook.CookId,
                        FoodCategoryId = model.CategoryId,
                        CuisineId = model.CuisineId,
                        CaloricityId = model.CaloricityId,
                        InsertDate = DateTime.UtcNow,
                        IsEnabled = true
                    };

                    DbContext.Recipes.Add(recipe);
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
