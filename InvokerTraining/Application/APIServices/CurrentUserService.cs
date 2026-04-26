using InvokerTraining.Application.Abstractions;
using System.Security.Claims;

namespace InvokerTraining.Application.APIServices
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        public Guid? CurrentUserID
        {
            get
            {
                var userId = _httpContextAccessor?.HttpContext?.User.FindFirstValue("userId");
                return userId == null ? null : Guid.Parse(userId);
            }
        }
    }
}
