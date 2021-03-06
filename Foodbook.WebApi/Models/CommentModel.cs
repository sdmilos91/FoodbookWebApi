﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Foodbook.WebApi.Models
{
    public class PostRecipeCommentModel
    {

        public int Rating { get; set; }

        [Required]
        public string CommentText { get; set; }

        public long RecipeId { get; set; }
        
        public DateTime InsertDate { get; set; }

    }
}