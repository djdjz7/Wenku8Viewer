using ReactiveUI;
using AngleSharp;
using System.Net.Http;
using AngleSharp.Dom;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Io;
using System.Web;
using System.Diagnostics;
using System.Text;
using System;
using Avalonia.Platform;
using System.Reactive;

namespace Wenku8Viewer.ViewModels;

public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    public LoginViewModel(IScreen screen)
    {
        HostScreen = screen;
        LoginCommand = ReactiveCommand.Create(Login, this.WhenAnyValue(
            x => x.Username,
            x => x.Password,
            (username, password) => !string.IsNullOrWhiteSpace(username) && !string.IsNullOrEmpty(password)
            ));
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

    public string? UrlPathSegment { get; } = Guid.NewGuid().ToString().Substring(0, 5);
    public IScreen HostScreen { get; }
    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public async void Login()
    {
        var data = new Dictionary<string, string>
        {
            {"username", HttpUtility.UrlEncode(Username) },
            {"password", HttpUtility.UrlEncode(Password) },
            {"usecookie", "86400" },
            {"action", "login" },
            {"submit", "%26%23160%3B%B5%C7%26%23160%3B%26%23160%3B%C2%BC%26%23160%3B" }
        };
        var response = await httpClient.PostAsync("https://www.wenku8.net/login.php?do=submit", new FormUrlEncodedContent(data));

        var responseContent = Encoding.GetEncoding("gb2312").GetString(await response.Content.ReadAsByteArrayAsync());
        if (!responseContent.Contains("登录成功"))
            return;
        var config = Configuration.Default.WithDefaultLoader().WithDefaultCookies();
        var address = "https://www.wenku8.net";
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
        HostScreen.Router.Navigate.Execute(new MainViewModel(HostScreen, context, true));
    }
}
