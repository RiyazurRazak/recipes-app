using System.ComponentModel.DataAnnotations;

namespace recipes_app.Models
{
    public class UsersModel
    {
        [Key]
        public string UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }


    }
}
