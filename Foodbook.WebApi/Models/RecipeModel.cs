using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foodbook.WebApi.Models
{
    public class RecipeModel
    {
        public long RecipeId { get; set; }
        public long CookId { get; set; }
        public string CookName { get; set; }
        public string Name { get; set; }
        public long CuisineId { get; set; }
        public string CuisineName { get; set; }
        public long CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string RecipeText { get; set; }
        public string VideoUrl { get; set; }
        public long? CaloricityId { get; set; }
        public string CaloricityName { get; set; }
        public DateTime InsertDate { get; set; }
        public List<RecipeCommentModel> Comments { get; set; }
        public double? Rating { get; set; }
        public int PreparationTime { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public List<PhotoModel> Photos { get; set; }
        public bool IsMine { get; set; }
        public bool IsFavourite { get; set; }
    }

    public class RecipeCommentModel
    {
        public string CookPhotoUrl;
        public long CommentId { get; set; }
        public int Rating { get; set; }
        public string CommentText { get; set; }
        public long CookId { get; set; }
        public string CookName { get; set; }
        public DateTime InsertDate { get; set; }

    }

    public class RequestRecipeModel
    {        

        public int StartIndex { get; set; }
        public string Text { get; set; }
        public long? CategoryId { get; set; }
        public long? CuisineId { get; set; }
        public bool JustsAllRecipes { get; set; }
    }

    public class ResponseRecipeModel
    {
        public ResponseRecipeModel()
        {
            MyRecipes = new List<RecipeModel>();
            FavouriteRecipes = new List<RecipeModel>();
            AllRecipes = new List<RecipeModel>();
        }

        public List<RecipeModel> MyRecipes { get; set; }
        public List<RecipeModel> FavouriteRecipes { get; set; }
        public List<RecipeModel> AllRecipes { get; set; }
    }

    public class PostRecipeModel
    {
        public string Name { get; set; }
        public long CuisineId { get; set; }
        public long CategoryId { get; set; }
        public string RecipeText { get; set; }
        public string VideoUrl { get; set; }
        public long? CaloricityId { get; set; }
        public int PreparationTime { get; set; }
        public List<PhotoModel> Photos { get; set; }
    }


    public class PhotoModel
    {
        public string Url { get; set; }

        public string Name { get; set; }

    }
}

