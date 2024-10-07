using Blazored.LocalStorage;
using Chat.Client;
using Chat.Client.Repositories;
using Chat.Client.Repositories.Contracts;
using Chat.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthHandler>();


builder.Services.AddScoped<StorageService>();


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = 
    new Uri("http://localhost:5068/") });


builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<IUserIntegration, UserIntegration>();
builder.Services.AddScoped<IChatIntegration, ChatIntegration>();


await builder.Build().RunAsync();
