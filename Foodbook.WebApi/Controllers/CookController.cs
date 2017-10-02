using Foodbook.DataAccess;
using Foodbook.WebApi.Models;
using Foodbook.WebApi.Utils;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Foodbook.WebApi.Controllers
{
    public class CookController : BaseController
    {
        [Authorize]
        public IHttpActionResult Get(long id)
        {
            try
            {
                string aspUserId = User.Identity.GetUserId();
                Cook cook = DbContext.Cooks.Find(id);

                if(cook == null)
                {
                    cook = DbContext.Cooks.FirstOrDefault(x => x.ApsUserId.Equals(aspUserId));
                }

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
                        PhotoUrl = string.IsNullOrEmpty(cook.PhotoUrl) ? "chefIcon.png" : cook.PhotoUrl,
                        Recipes = cook.Recipes.ToList().Select(x => InitRecipeModel(x)).ToList(),
                        FavouriteRecipes = cook.Recipes1.ToList().Select(x => InitRecipeModel(x)).ToList(),
                        FullName = cook.FirstName + " " + cook.LastName,
                        FollowedCooks = cook.Cooks.Select(x => new CookModel
                        {
                            CookId = x.CookId,
                            Biography = x.Bio,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            PhotoUrl = x.PhotoUrl
                        }).ToList(),
                        Comments = cook.CookComments.ToList().Select(z => new CookCommentModel
                        {
                            CommentId = z.CommentId,
                            CookId = z.CommentOwnerId,
                            CookName = string.Join(" ", z.Cook1.FirstName, z.Cook1.LastName),
                            CommentText = z.CommentText,
                            InsertDate = z.DateInserted,
                            Rating = z.Rating,
                            CookPhotoUrl = string.IsNullOrEmpty(z.Cook1.PhotoUrl) ? "chefIcon.png" : z.Cook.PhotoUrl

                        }).ToList(),
                        NumberOfRecipes = cook.Recipes.Count,
                        NumberOfFollowers = cook.Cooks1.Count,
                        Rating = CalculateCookRating(cook.CookComments)
                    };

                    return Ok(model);
                }                
                
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        public IHttpActionResult Get()
        {
            try
            {            
                string aspUserId = User.Identity.GetUserId();

                List<ResponseCookModel> model = DbContext.Cooks.Where(x => x.ApsUserId != aspUserId).ToList().Select(cook => new ResponseCookModel
                {
                    CookId = cook.CookId,
                    Biography = cook.Bio,
                    FirstName = cook.FirstName,
                    LastName = cook.LastName,
                    PhotoUrl = string.IsNullOrEmpty(cook.PhotoUrl) ? "chefIcon.png" : cook.PhotoUrl,
                    Recipes = cook.Recipes.ToList().Select(x => InitRecipeModel(x)).ToList(),
                    FavouriteRecipes = cook.Recipes1.ToList().Select(x => InitRecipeModel(x)).ToList(),
                    IsFollowed = !cook.ApsUserId.Equals(aspUserId) && cook.Cooks1.Any(x => x.ApsUserId.Equals(aspUserId)),
                    FullName = cook.FirstName + " " + cook.LastName,
                    FollowedCooks = cook.Cooks.Select(x => new CookModel
                    {
                        CookId = x.CookId,
                        Biography = x.Bio,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        PhotoUrl = x.PhotoUrl
                    }).ToList(),
                    Comments = cook.CookComments.ToList().Select(z => new CookCommentModel
                    {
                        CommentId = z.CommentId,
                        CookId = z.CommentOwnerId,
                        CookName = string.Join(" ", z.Cook1.FirstName, z.Cook1.LastName),
                        CommentText = z.CommentText,
                        InsertDate = z.DateInserted,
                        Rating = z.Rating,
                        CookPhotoUrl = string.IsNullOrEmpty(z.Cook1.PhotoUrl) ? "chefIcon.png" : z.Cook1.PhotoUrl

                    }).ToList(),
                    NumberOfRecipes = cook.Recipes.Count,
                    NumberOfFollowers = cook.Cooks1.Count,
                    Rating = CalculateCookRating(cook.CookComments)
                }).ToList();

                return Ok(model);
                

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
                Rating = CalculateRecipeRating(x.RecipeComments),
                RecipeText = x.RecipeText,
                PreparationTime = x.PreparationTime,
                ProfilePhotoUrl = x.RecipeImages.Any() ? x.RecipeImages.FirstOrDefault().PhotoUrl : "recipePlaceholder.png",
                IsMine = x.Cook.ApsUserId.Equals(aspUserId),
                IsFavourite = x.Cooks.Any(z => z.ApsUserId.Equals(aspUserId)),
                Ingredients = string.IsNullOrEmpty(x.Ingredients) ? new List<IngredientModel>() : (List<IngredientModel>)Newtonsoft.Json.JsonConvert.DeserializeObject(x.Ingredients, typeof(List<IngredientModel>)),
                Comments = x.RecipeComments.ToList().Select(z => new RecipeCommentModel
                {
                    CommentId = z.CommentId,
                    CookId = z.CookId,
                    CookName = string.Join(" ", z.Cook.FirstName, z.Cook.LastName),
                    CommentText = z.CommentText,
                    InsertDate = z.DateInserted,
                    Rating = z.Rating,
                    CookPhotoUrl = string.IsNullOrEmpty(z.Cook.PhotoUrl) ? "chefIcon.png" : z.Cook.PhotoUrl

                }).ToList(),
                Photos = x.RecipeImages.Select(z => new PhotoModel
                {
                    Url = z.PhotoUrl
                }).ToList()
            };
        }

        private double? CalculateCookRating(ICollection<CookComment> comments)
        {
            if (comments.Any())
            {
                var groupedComments = comments.GroupBy(x => x.CommentOwnerId);
                List<double> rating = new List<double>();
                foreach (var item in groupedComments)
                {
                    rating.Add(item.Average(x => x.Rating));
                }

                return Math.Round(rating.Average(), 2);
            }

            return null;
        }

        private double? CalculateRecipeRating(ICollection<RecipeComment> comments)
        {
            if (comments.Any())
            {
                var groupedComments = comments.GroupBy(x => x.CookId);
                List<double> rating = new List<double>();
                foreach (var item in groupedComments)
                {
                    rating.Add(item.Average(x => x.Rating));
                }

                return Math.Round(rating.Average(), 2);
            }

            return null;
        }
    }
}
