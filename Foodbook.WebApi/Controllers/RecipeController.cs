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

        public IHttpActionResult Get([FromUri]RequestRecipeModel requestModel)
        {
            ResponseRecipeModel responseModel = new ResponseRecipeModel();
            try
            {
                if (requestModel != null)
                {
                    if (User.Identity.IsAuthenticated && !requestModel.JustsAllRecipes)
                    {
                        string aspUserId = User.Identity.GetUserId();

                        List<Recipe> myRecipes = DbContext.Recipes.Where(x => x.Cook.ApsUserId.Equals(aspUserId) && (string.IsNullOrEmpty(requestModel.Text) ? true : x.Name.ToLower().Contains(requestModel.Text.ToLower()))
                                                                     && (requestModel.CategoryId.HasValue ? x.FoodCategoryId == requestModel.CategoryId.Value : true)
                                                                     && (requestModel.CuisineId.HasValue ? x.CuisineId == requestModel.CuisineId.Value : true)
                                                                    ).OrderByDescending(x => x.RecipeComments.Average(z => z.Rating)).ToList();

                        List<Recipe> favRecipes = DbContext.Recipes.Where(x => x.Cooks.Any(z => z.ApsUserId.Equals(aspUserId)) && (string.IsNullOrEmpty(requestModel.Text) ? true : x.Name.ToLower().Contains(requestModel.Text.ToLower()))
                                                                     && (requestModel.CategoryId.HasValue ? x.FoodCategoryId == requestModel.CategoryId.Value : true)
                                                                     && (requestModel.CuisineId.HasValue ? x.CuisineId == requestModel.CuisineId.Value : true)
                                                                    ).OrderByDescending(x => x.RecipeComments.Average(z => z.Rating)).ToList();

                        responseModel.MyRecipes = myRecipes.Select(x => InitRecipeModel(x)).ToList();
                        responseModel.FavouriteRecipes = favRecipes.Select(x => InitRecipeModel(x)).ToList();
                    }

                    List<Recipe> recipes = DbContext.Recipes.Where(x => (string.IsNullOrEmpty(requestModel.Text) ? true : x.Name.ToLower().Contains(requestModel.Text.ToLower()))
                                                                     && (requestModel.CategoryId.HasValue ? x.FoodCategoryId == requestModel.CategoryId.Value : true)
                                                                     && (requestModel.CuisineId.HasValue ? x.CuisineId == requestModel.CuisineId.Value : true)
                                                                    ).OrderByDescending(x => x.RecipeComments.Average(z => z.Rating)).Skip(requestModel.StartIndex).ToList();

                    responseModel.AllRecipes = recipes.Select(x => InitRecipeModel(x)).ToList();

                    return Ok(responseModel);
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
                        IsEnabled = true,
                        PreparationTime = model.PreparationTime
                    };

                    recipe.RecipeImages.Clear();

                    foreach (var item in model.Photos)
                    {                        
                        recipe.RecipeImages.Add(new RecipeImage
                        {
                            InsertDate = DateTime.Now,
                            PhotoUrl = item.Url
                        });
                    }

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

        [Authorize]
        public IHttpActionResult Put(long id, [FromBody] PostRecipeModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string aspUserId = User.Identity.GetUserId();

                    Cook cook = DbContext.Cooks.FirstOrDefault(x => x.ApsUserId.Equals(aspUserId));
                    Recipe recipe = DbContext.Recipes.Find(id);

                    if (recipe != null)
                    {
                        recipe.Name = model.Name;
                        recipe.RecipeText = model.RecipeText;
                        recipe.FoodCategoryId = model.CategoryId;
                        recipe.CuisineId = model.CuisineId;
                        recipe.CaloricityId = model.CaloricityId;
                        recipe.IsEnabled = true;
                        recipe.PreparationTime = model.PreparationTime;

                        var recipeImages = DbContext.RecipeImages.Where(x => x.RecipeId == recipe.RecipeId);
                        foreach (var img in recipeImages)
                        {
                            DbContext.RecipeImages.Remove(img);
                        }
                        DbContext.SaveChanges();


                        foreach (var item in model.Photos)
                        {
                            recipe.RecipeImages.Add(new RecipeImage
                            {
                                InsertDate = DateTime.Now,
                                PhotoUrl = item.Url
                            });
                        }

                        DbContext.SaveChanges();

                        return Ok();
                    }else
                    {
                        return BadRequest();
                    }
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

        [Authorize]
        public IHttpActionResult Delete(long id)
        {
            try
            {
                Recipe recipe = DbContext.Recipes.Find(id);
                if (recipe != null)
                {
                    DbContext.Recipes.Remove(recipe);
                    DbContext.SaveChanges();
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }

        private RecipeModel InitRecipeModel(Recipe x)
        {
            string aspUserId = User.Identity.GetUserId();

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
                CuisineName = x.Cuisine.CuisineName,
                CookName = string.Join(" ", x.Cook.FirstName, x.Cook.LastName),
                VideoUrl = x.VideoUrl,
                InsertDate = x.InsertDate,
                Rating = x.RecipeComments.Any() ? (double?)x.RecipeComments.Sum(z => z.Rating) / x.RecipeComments.Count() : null,
                RecipeText = x.RecipeText,
                PreparationTime = x.PreparationTime,
                ProfilePhotoUrl = x.RecipeImages.Any() ? x.RecipeImages.FirstOrDefault().PhotoUrl : "http://kuhinjarecepti.com/wp-content/uploads/2012/01/%C5%A0opska-salata.jpeg",
                IsMine = x.Cook.ApsUserId.Equals(aspUserId),
                IsFavourite = x.Cooks.Any(z => z.ApsUserId.Equals(aspUserId)),

                Comments = x.RecipeComments.ToList().Select(z => new RecipeCommentModel
                {
                    CommentId = z.CommentId,
                    CookId = z.CookId,
                    CookName = string.Join(" ", z.Cook.FirstName, z.Cook.LastName),
                    CommentText = z.CommentText,
                    InsertDate = z.DateInserted,
                    Rating = z.Rating

                }).ToList(),
                Photos = x.RecipeImages.Select(z => new PhotoModel
                {
                    Url = z.PhotoUrl
                }).ToList()
            };
        }
    }
}
