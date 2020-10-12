using System.Collections.Generic;

using Models;

namespace WEB.Models.ViewModels
{
    public class NCWUViewModel
    {
        public New New { get; set; }
        public List<Comment> Comments { get; set; }
        public string WebUserId { get; set; }
        public string UserName { get; set; }
        public string ReturnUrl { get; set; }
    }
}
