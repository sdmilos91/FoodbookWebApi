using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foodbook.WebApi.Models
{

    public class CookModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Biography { get; set; }

        public string PhotoUrl { get; set; }

        public long CookId { get; set; }

    }

    public class ResponseCookModel : CookModel
    {

        public List<CookModel> FollowedCooks { get; set; }
        public List<RecipeModel> Recipes { get; set; }
        public List<RecipeModel> FavouriteRecipes { get; set; }

    }
}