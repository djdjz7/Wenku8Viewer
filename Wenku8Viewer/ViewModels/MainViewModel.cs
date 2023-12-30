using AngleSharp.Dom;
using AngleSharp;
using ReactiveUI;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Wenku8Viewer.ViewModels;

public class MainViewModel : ViewModelBase
{
    private bool isLoggedIn = false;
    public bool IsLoggedIn
    {
        get => isLoggedIn;
        set => this.RaiseAndSetIfChanged(ref isLoggedIn, value);
    }

    private HttpClient httpClient = new HttpClient();
    private string username = string.Empty;
    private string password = string.Empty;
    public string Username
    {
        get => username;
        set => this.RaiseAndSetIfChanged(ref username, value);
    }

    public string Password
    {
        get => password;
        set => this.RaiseAndSetIfChanged(ref password, value);
    }

    public async void Login()
    {
        var data = new Dictionary<string, string>
        {
            {"username", HttpUtility.UrlEncode(Username) },
            {"password", HttpUtility.UrlEncode(Password) },
            {"usecookie", "0" },
            {"action", "login" },
            {"submit", "%26%23160%3B%B5%C7%26%23160%3B%26%23160%3B%C2%BC%26%23160%3B" }
        };
        var response = await httpClient.PostAsync("https://www.wenku8.net/login.php?do=submit", new FormUrlEncodedContent(data));

        var responseContent = Encoding.GetEncoding("gb2312").GetString(await response.Content.ReadAsByteArrayAsync());
        if (!responseContent.Contains("登录成功"))
            return;

        isLoggedIn = true;
        var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
        var address = "https://www.wenku8.net/index.php";
        var context = BrowsingContext.New(config);
        foreach (var header in response.Headers)
        {
            if (header.Key.ToLower() == "set-cookie")
            {
                foreach (var item in header.Value)
                {
                    context.SetCookie(new Url(address), item);
                }
            }
        }
        var document = await context.OpenAsync(address);
    }
}
