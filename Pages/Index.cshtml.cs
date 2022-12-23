using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CS58___Entity_Framework.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly ArticleContext context;

    public IndexModel(ILogger<IndexModel> logger, ArticleContext context)
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
