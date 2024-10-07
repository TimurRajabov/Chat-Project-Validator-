using System.Security.Claims;

namespace Chat.Api.Helpers;

public class UserHelper(IHttpContextAccessor contextAccessor)
{
    private readonly IHttpContextAccessor _contextAccessor = contextAccessor;

    public Guid GetUserId()
    {
        var userId = Guid.Parse(_contextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));
        return userId;

    }
}