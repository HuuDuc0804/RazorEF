using Microsoft.AspNetCore.Authorization;

namespace Security.Requirements
{
    public class ArticleUpdateRequirement : IAuthorizationRequirement
    {
      public ArticleUpdateRequirement(int year = 2021, int month = 6, int day = 1)
      {
            Year = year;
            Month = month;
            Day = day;
        }

        public int Year { get; }
        public int Month { get; }
        public int Day { get; }
    }
}

