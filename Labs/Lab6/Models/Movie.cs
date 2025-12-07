using System.ComponentModel.DataAnnotations;

namespace Lab6.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [MaxLength(50, ErrorMessage = "Tytuł może mieć maksymalnie 50 znaków")]
        public string Title { get; set; } = string.Empty;

        [UIHint("LongText")]
        public string? Description { get; set; }

        [UIHint("Stars")]
        [Range(1, 5, ErrorMessage = "Ocena filmu musi być liczbą pomiędzy 1 a 5")]
        public int Rating { get; set; }

        public string? TrailerLink { get; set; }

        public int GenreId { get; set; }

        public Genre? Genre { get; set; }
    }
}
