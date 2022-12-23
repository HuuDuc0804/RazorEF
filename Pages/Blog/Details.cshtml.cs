using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CS58___Entity_Framework.Pages_Blog
{
    public class DetailsModel : PageModel
    {
        private readonly ArticleContext _context;

        public DetailsModel(ArticleContext context)
        {
            _context = context;
        }

      public Article Article { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            else 
            {
                Article = article;
            }
            return Page();
        }
    }
}
