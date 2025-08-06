namespace WebSIMS.Services.Interfaces;

public interface ICookiesService
{
    void SetCookie(string key, string value, bool isPersistent = false);
    string GetCookie(string key);
    void DeleteCookie(string key);
    Dictionary<string, string> GetAllCookies();
}