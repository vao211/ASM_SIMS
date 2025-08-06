using WebSIMS.Repository;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Services;

public class CookiesService : ICookiesService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CookiesService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetCookie(string key, string value, bool isPersistent = false)
    {
        var options = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = isPersistent ? DateTimeOffset.UtcNow.AddDays(30) : null
        };
        _httpContextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
    }

    public string GetCookie(string key)
    {
        return _httpContextAccessor.HttpContext.Request.Cookies[key];
    }

    public void DeleteCookie(string key)
    {
        _httpContextAccessor.HttpContext.Response.Cookies.Delete(key);
    }

    public Dictionary<string, string> GetAllCookies()
    {
        return _httpContextAccessor.HttpContext.Request.Cookies.ToDictionary(x => x.Key, x => x.Value);
    }
    
}