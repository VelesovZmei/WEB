using System.ComponentModel.DataAnnotations;

namespace WEB.Models.ViewModels
{
    public class OptionsViewModel
    {
        [Url]
        public string TYTBY { get; set; }

        [Url]
        public string S13 { get; set; }
        
        [Url] 
        public string ONLINER { get; set; }

        public double CheckingInterval { get; set; }
}
}
