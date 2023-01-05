using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS58___Entity_Framework.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; } = default!;

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int CurrentPage { get; set; }

        [BindProperty]
        public int CountPages { get; set; }

        public async Task OnGetAsync(string SearchString)
        {
            if (_context.Articles != null)
            {
                int totalArticle;
                List<Article> _article;

                if (string.IsNullOrEmpty(SearchString))
                {
                    _article = await _context.Articles.ToListAsync();
                    totalArticle = _article.Count();
                }
                else
                {
                    _article = await _context.Articles.Where(x => x.Title.Contains(SearchString)).ToListAsync();
                    totalArticle = _article.Count();
                }

                CountPages = (int)Math.Ceiling((double)totalArticle / (double)ITEMS_PER_PAGE);
                if (CurrentPage < 1)
                    CurrentPage = 1;
                if (CurrentPage > CountPages)
                    CurrentPage = CountPages;

                Article = _article.Skip((CurrentPage - 1) * ITEMS_PER_PAGE)
                                    .Take(ITEMS_PER_PAGE).ToList();
            }
        }
    }
}
