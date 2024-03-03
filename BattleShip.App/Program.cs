using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BattleShip.App;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("ServerAPI",
      client => client.BaseAddress = new Uri(builder.Configuration["ApiHost"]))
    .AddHttpMessageHandler(sp => {
        var httpMessageHandler = sp.GetService<AuthorizationMessageHandler>()?
            .ConfigureHandler(authorizedUrls: [builder.Configuration["ApiHost"]]);
        return httpMessageHandler ?? throw new NullReferenceException(nameof(AuthorizationMessageHandler));
    });

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
  .CreateClient("ServerAPI"));
builder.Services.AddOidcAuthentication(options =>
{
  builder.Configuration.Bind("Auth0", options.ProviderOptions);
  options.ProviderOptions.ResponseType = "code";
  options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]);
});

await builder.Build().RunAsync();
