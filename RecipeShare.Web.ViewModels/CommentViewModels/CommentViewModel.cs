﻿using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RecipeShare.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Web.ViewModels.CommentViewModels
{
    public class CommentViewModel
    {
        public required Guid Id { get; set; }
        public required string DateOfRelease { get; set; }
        public required string Text { get; set; }
        public required string UserName { get; set; }
        public List<CommentViewModel> Responses { get; set; } = new List<CommentViewModel>();
        public required bool IsResponse { get; set; } = false;
    }
}
