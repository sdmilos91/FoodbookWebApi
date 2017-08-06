using Foodbook.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Foodbook.WebApi.Controllers
{
    public class RecipeCommonDataController : BaseController
    {
        public IHttpActionResult Get()
        {
            try
            {
                RecipeCommonDataModel model = new RecipeCommonDataModel
                {
                    Caloricities = DbContext.Caloricities.Select(x => new CaloricityModel
                    {
                        CaloricityId = x.CaloricityId,
                        MaxValue = x.MaxValue,
                        MinValue = x.MinValue,
                        Name = x.Name
                    }).ToList(),
                    Categories = DbContext.FoodCategories.Where(x => x.IsActive).Select(x => new FoodCategoryModel
                    {
                        CategoryId = x.CategoryId,
                        CategoryName = x.CategoryName
                    }).ToList(),
                    Cuisines = DbContext.Cuisines.Where(x => x.IsActive).Select(x => new CuisineModel
                    {
                        CuisineId = x.CuisineId,
                        CuisineName = x.CuisineName
                    }).ToList()
                };

                return Ok(model);


            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
