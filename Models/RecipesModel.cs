using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace recipes_app.Models
{
    public class RecipesModel
    {
        [Key]
        public string RecipeId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("Author")]
        public string AuthorId { get; set; }

        public UsersModel Author { get; set; }

    }
}
