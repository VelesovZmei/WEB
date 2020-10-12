using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Afinn
    {
        public Afinn()
        {
            Culture = "ru-BY";
        }

        public Afinn(string culture)
        {
            Culture = culture;
        }

        [Required, StringLength(64)]
        public string Word { get; set; }

        [Required, StringLength(5, MinimumLength = 5)]
        public string Culture { get; set; }

        public int Rate { get; set; }
    }
}
