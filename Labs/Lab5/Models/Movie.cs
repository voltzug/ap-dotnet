using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Lab5.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        public required string Title { get; set; }

        [UIHint("LongText")]
        public string? Description { get; set; }

        [Range(0, 5, ErrorMessage = "Ocena musi być liczbą od 0 do 5.")]
        [UIHint("Stars")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Link do zwiastuna jest wymagany.")]
        [Display(Name = "Link do zwiastuna")]
        [TrailerLinkValidation(ErrorMessage = "Podaj poprawny adres URL zaczynający się od http:// lub https://, bez spacji i niedozwolonych znaków.")]
        public required string TrailerLink { get; set; }
    }

    public class TrailerLinkValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is not string url || string.IsNullOrWhiteSpace(url))
                return false;

            // Must start with http:// or https:// and contain only allowed URL characters (basic check)
            var pattern = @"^https?:\/\/[^\s""'<>{}\[\]\\^`|]+$";
            return Regex.IsMatch(url, pattern, RegexOptions.IgnoreCase);
        }
    }
}
