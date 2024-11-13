using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using static RecipeShare.Common.ApplicationConstants;
using static RecipeShare.Common.EntityValidationMessages;

namespace RecipeShare.Data.Models
{
    public class Comment
    {
        public Comment()
        {
            Id = Guid.NewGuid();
        }

        [Key]
        [Comment("The Id of the Comment")]
        public Guid Id { get; set; }

        [Required]
        [Comment("The Date of release of the Comment")]
        [RegularExpression(RegexDateTimePattern, ErrorMessage = ErrorMessageDate)]
        public DateTime DateOfRelease { get; set; }

        [Required]
        [Comment("The text of the Comment")]
        [StringLength(CommentTextMaxLen, ErrorMessage = ErrorMessageCommentText)]
        public string Text { get; set; } = null!;

        [Required]
        [Comment("The Id of the Recipe")]
        public Guid RecipeId { get; set; }

        [Required]
        [Comment("The Recipe to which the Comment was posted")]
        [ForeignKey(nameof(RecipeId))]
        public virtual Recipe Recipe { get; set; } = null!;

        [Required]
        [Comment("The Id of the User")]
        public Guid UserId { get; set; }

        [Required]
        [Comment("The User who has created the comment")]
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
