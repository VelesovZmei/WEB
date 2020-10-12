using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using WEB.Data;
using WEB.Models.ViewModels;

namespace WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class CountersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CountersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/counters
        [HttpGet]
        public async Task<CounterViewModel> GetCounter()
        {
            var result = new CounterViewModel();
            result.Users = await _context.WebUsers.CountAsync().ConfigureAwait(false);
            result.Comments = await _context.Comments.CountAsync().ConfigureAwait(false);
            result.News = await _context.News.CountAsync().ConfigureAwait(false);
            return result;
        }
    }
}
