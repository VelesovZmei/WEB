using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Hangfire;

using Models;
using WEB.Data;
using WEB.Extensions;
using WEB.Models.ViewModels;
using WEB.NLP.Texterra;
using WEB.RSS;

namespace WEB.Controllers
{
    public class NewsController : Controller
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ApplicationDbContext _context;
        private readonly RssReadService _rssReader;
        private readonly UserManager<WebUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<NewsController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public NewsController(
            IHttpContextAccessor httpContext,
            ApplicationDbContext context,
            UserManager<WebUser> userManager,
             RssReadService rssReader,
             IConfiguration configuration,
             IHttpClientFactory clientFactory,
             ILogger<NewsController> logger)
        {
            _httpContext = httpContext;
            _context = context;
            _userManager = userManager;
            _rssReader = rssReader;
            _configuration = configuration;
            _logger = logger;
            _clientFactory = clientFactory;
        }


        // GET: News
        public IActionResult Index()
        {
            return View(_context.News.OrderByDescending(x => x.DatePosted).ToList());
        }

        //[HttpGet]
        //public async Task<IActionResult> LoadNews()
        //{
        //    await _newsService.LoadNewsInDb();
        //    return RedirectToAction("NewsList");
        //}

        // GET: News/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var VM = new NCWUViewModel();

            VM.New = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);

            if (VM.New == null)
            {
                return NotFound();
            }

            VM.Comments = _context.Comments
                .Where(i => i.NewId == id && i.Moderation)
                .OrderByDescending(t => t.Date)
                .Take(5)
                .ToList();

            var user = await _userManager.GetUserAsync(User);
            //ViewData["WebUserId"] = _context.Users.Where(u=>u.UserName==User.Identity.Name).Select(s=>s.Id).FirstOrDefault();
            VM.WebUserId = user?.Id;
            VM.UserName = user?.UserName;
            VM.ReturnUrl = _httpContext.HttpContext.Request.GetEncodedPathAndQuery();
            // or
            //VM.ReturnUrl = Url.Action("Details", null, null, null);
            ViewData["UnpublicContent"] = _context.Comments
                .Where(i => i.NewId == id && !i.Moderation && i.WebUserId == VM.WebUserId)
                .Select(c => c.Content)
                .FirstOrDefault();
            return View(VM);
        }

        // GET: News/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("ID,Head,Text,SourceURL,Author,DatePosted")] New @new)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@new);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@new);
        }

        // GET: News/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @new = await _context.News.FindAsync(id);
            if (@new == null)
            {
                return NotFound();
            }
            return View(@new);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Head,Text,SourceURL,Author,DatePosted")] New @new)
        {
            if (id != @new.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@new);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NewExists(@new.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@new);
        }

        // GET: News/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @new = await _context.News
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@new == null)
            {
                return NotFound();
            }

            return View(@new);
        }

        private bool NewExists(string id)
        {
            return _context.News.Any(e => e.Id == id);
        }

        public async Task<IActionResult> LoadRSS()
        {
            var options = new OptionsViewModel();
            _configuration.Bind("Rss", options);
            _configuration.Bind("Rss:Urls", options);
            await LoadNewsInDb(options.TYTBY, Parsing.TutByParseNews, options.CheckingInterval + 0.017);
            //await LoadNewsInDb(options.S13, Parsing.S13ParseNews, options.CheckingInterval + 0.017);
            await LoadNewsInDb(options.ONLINER, Parsing.OnlinerParseNews, options.CheckingInterval + 0.017);
            RecurringJob.AddOrUpdate(() => this.LoadRSS(), Cron.Hourly);
            return RedirectToAction(nameof(Index));
        }

        public delegate string FuncParsing(string uri);

        public async Task LoadNewsInDb(string url, FuncParsing funcParsing, double period)
        {
            _logger.LogWarning("LoadNewsInDb called with {0}", url);

            var newsItems = _rssReader.ReadRss(url, period); //Get rss feed

            if (!(newsItems?.Count > 0))
            {
                await Task.CompletedTask;
            }

            var parsedNewsList = new List<New>(newsItems.Count);

            // Prepare sorted dictionary for Texterra response matching
            var dictionary = await _context.Afinns
                .AsNoTracking()
                .Where(a => a.Culture == "ru-BY")
                .OrderBy(a => a.Word)
                .ToDictionaryAsync<Afinn, string, int>(a => a.Word, a => a.Rate);

            foreach (var item in newsItems)
            {
                if (item == null || _context.News.Any(n => n.SourceURL == item.Id))
                {
                    continue;
                }

                var parsedNew = new New()
                {
                    Head = item.Title.Text,
                    DatePosted = item.PublishDate.ToUnixTimeSeconds(),
                    SourceURL = item.Id
                };

                if (item.Authors?.Count > 0)
                {
                    parsedNew.Author = item.Authors.First()?.Name;
                }

                parsedNew.Author ??= "Author Unknown";

                parsedNew.Text = funcParsing(item.Links.FirstOrDefault()?.Uri.AbsoluteUri).CleanHtml();

                if (!string.IsNullOrEmpty(parsedNew.Text))
                {
                    parsedNewsList.Add(parsedNew);  //Add model's object to list
                }
            }

            if (parsedNewsList.Count == 0)
            {
                await Task.CompletedTask;
            }

            // Prepare request to Texterra
            var forTexterra = JsonSerializer.Serialize<List<New>>(parsedNewsList);
            _logger.LogWarning("Sent request to Texterra: {0}", parsedNewsList.Count);

            // Do request and wait response
            var lemmas = await NlpHelper
                .GetLemmasAsync(_configuration["TexterraApiKey"], forTexterra, _clientFactory)
                .ConfigureAwait(false);

            _logger.LogWarning("Received Texterra response: {0}",
                lemmas != null ? lemmas.Length.ToString() : "null");

            if (lemmas?.Length > 2)
            {
                try
                {
                    // Convert response to object list
                    var response = JsonSerializer.Deserialize<List<Respond>>(lemmas);
                    // Rate each news
                    foreach (var item in response)
                    {
                        var news = parsedNewsList.Find(n =>
                            string.Equals(n.Text, item.text, StringComparison.OrdinalIgnoreCase));
                        if (news != null)
                        {
                            news.Score = item.RateIt(dictionary);
                        }
                    }
                }
                catch (JsonException jx)
                {
                    _logger.LogError("Texterra JSON response", jx.Message);
                }
            }

            //Add list to database
            _context.News.AddRange(parsedNewsList);
            await _context.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
