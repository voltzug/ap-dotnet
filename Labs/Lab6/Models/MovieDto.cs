using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab6.Models
{
    public class MovieDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany")]
        [MaxLength(50, ErrorMessage = "Tytuł może mieć maksymalnie 50 znaków")]
        public string Title { get; set; } = string.Empty;

        [UIHint("LongText")]
        public string? Description { get; set; }

        [UIHint("Stars")]
        [Range(1, 5, ErrorMessage = "Ocena filmu musi być liczbą pomiędzy 1 a 5")]
        public int Rating { get; set; }

        [RegularExpression(@"^https?:\/\/[^\s""'<>{}\[\]\\^`|]+$", ErrorMessage = "Link do zwiastunu musi być poprawnym adresem URL zaczynającym się od http:// lub https://")]
        public string? TrailerLink { get; set; }

        [Required(ErrorMessage = "Gatunek jest wymagany")]
        [MinLength(1, ErrorMessage = "Gatunek jest wymagany")]
        public string Genre { get; set; } = string.Empty;

        public List<string>? AllGenres { get; set; }
    }
}