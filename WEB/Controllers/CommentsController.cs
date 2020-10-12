using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Models;
using WEB.Data;

namespace WEB.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comments
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comments.Include(c => c.New).Include(c => c.WebUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.New)
                .Include(c => c.WebUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        //public IActionResult Create(string myuserid, string NewId, string content)
        //{
        //    var comment = new Comment(myuserid, NewId) 
        //    { 
        //        Content = content
        //    };

        //    return View(comment);
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content, string userName, string userId, string newId)
        //  public async Task<IActionResult> Create([Bind("Id,content,ConcurrencyStamp,Date,Moderation,userName,myserid,NewId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var comment = _context.Comments
                    .AsNoTracking()
                    .Where(i => i.NewId == newId && !i.Moderation && i.WebUserId == userId)
                    .Select
                    (c => new Comment()
                    { Id = c.Id, Content = c.Content, ConcurrencyStamp = c.ConcurrencyStamp }
                    ).FirstOrDefault();
                if (comment == null)
                {
                    comment = new Comment(userId, newId)
                    {
                        Content = content,
                        UserName = userName,
                    };
                    _context.Add(comment);
                }
                else if (!string.Equals(comment.Content, content, StringComparison.Ordinal))
                {
                    _context.Entry(comment).State = EntityState.Unchanged; //tracking on
                    comment.Content = content;
                    comment.Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "News", new { id = comment.NewId });
            }
            //ViewData["NewId"] = new SelectList(_context.News, "Id", "Author", comment.NewId);
            // ViewData["WebUserId"] = new SelectList(_context.Users, "Id", "Id", comment.WebUserId);
            //return View("~/News/Details.cshtml");
            return View("/Views/News/Details.cshtml");
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            //ViewData["NewId"] = new SelectList(_context.News, "ID", "Author", comment.NewId);
            //ViewData["WebUserId"] = new SelectList(_context.Users, "Id", "Id", comment.WebUserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(/*int id,*/ [FromForm] Comment comment)
        {
            //if (id != comment.Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
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
            //ViewData["NewId"] = new SelectList(_context.News, "ID", "Author", comment.NewId);
            // ViewData["WebUserId"] = new SelectList(_context.Users, "Id", "Id", comment.WebUserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.New)
                .Include(c => c.WebUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
