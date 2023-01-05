using Microsoft.AspNetCore.Authorization;

namespace Security.Requirements
{
    public class GenZRequirement : IAuthorizationRequirement
    {
        public GenZRequirement(int FromYear= 1997, int ToYear= 2012)
        {
            this.FromYear = FromYear;
            this.ToYear = ToYear;
        }

        public int FromYear { get; }
        public int ToYear { get; }
    }
}

