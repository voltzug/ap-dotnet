using System.ComponentModel.DataAnnotations;

namespace Lab6.Models
{
    public class Genre
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa gatunku jest wymagana")]
        public string Name { get; set; } = string.Empty;
    }
}