using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Foodbook.WebApi.Models
{
    public class RecipeCommonDataModel
    {
        public  List<FoodCategoryModel> Categories { get; set; }
        public List<CuisineModel> Cuisines { get; set; }
        public List<CaloricityModel> Caloricities { get; set; }

    }

    public class FoodCategoryModel
    {

        public long CategoryId { get; set; }
        public string CategoryName { get; set; }

    }

    public class CuisineModel
    {

        public long CuisineId { get; set; }
        public string CuisineName { get; set; }

    }

    public class CaloricityModel
    {

        public long CaloricityId { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string Name { get; set; }

       
    }
}