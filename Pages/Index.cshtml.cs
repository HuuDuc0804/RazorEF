using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS58___Entity_Framework.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly AppDbContext context;

    public IndexModel(ILogger<IndexModel> logger, AppDbContext context)
    {
        _logger = logger;
        this.context = context;
    }

    public void OnGet()
    {
        var posts = (from a in context.Articles
                    orderby a.Created descending
                    select a).ToList();
        ViewData["posts"] = posts;
    }
}
