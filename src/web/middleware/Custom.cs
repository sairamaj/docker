using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Authentication;
using Newtonsoft.Json.Linq;
using WebApplication;
public static class CustomAccountAppBuilderExtensions
{
    public static IApplicationBuilder UseFiservAccountAuthentication(this IApplicationBuilder app, CustomAccountOptions option)
    {
        return UseMiddlewareExtensions.UseMiddleware<CustomAccountMiddleware>(app, new object[]
        {
            Options.Create<CustomAccountOptions>(option)
        });
    }

}

public static class CustomAccountDefaults
{
    public const string AuthenticationScheme = "Fiserv";
    public static readonly string AuthorizationEndpoint = "http://localhost:5001/auth";
    public static readonly string TokenEndpoint = "http://localhost:5001/token";
    public static readonly string UserInformationEndpoint = "http://localhost:5001/user";
}

public class CustomAccountOptions : OAuthOptions
{
    public CustomAccountOptions()
    {
        AuthenticationScheme = CustomAccountDefaults.AuthenticationScheme;
        DisplayName = AuthenticationScheme;
        CallbackPath = new PathString("/signin-fiserv");
        AuthorizationEndpoint = CustomAccountDefaults.AuthorizationEndpoint;
        TokenEndpoint = CustomAccountDefaults.TokenEndpoint;
        UserInformationEndpoint = CustomAccountDefaults.UserInformationEndpoint;
        base.Scope.Add("https://graph.microsoft.com/user.read");

        System.Console.WriteLine("---------------------");
        if (this.StateDataFormat == null)
        {
            Console.WriteLine("statedataformat is null.");
        }
        else
        {
            Console.WriteLine("StateDataFormat:" + this.StateDataFormat);
        }
        System.Console.WriteLine("---------------------");
    }
}

public class CustomAccountMiddleware : OAuthMiddleware<CustomAccountOptions>
{
    public CustomAccountMiddleware(
           RequestDelegate next,
           IDataProtectionProvider dataProtectionProvider,
           ILoggerFactory loggerFactory,
           UrlEncoder encoder,
           IOptions<SharedAuthenticationOptions> sharedOptions,
           IOptions<CustomAccountOptions> options) :
           base(next, dataProtectionProvider, loggerFactory, encoder, sharedOptions, options)

    {
    }

    protected override AuthenticationHandler<CustomAccountOptions> CreateHandler()
    {
        //return new MicrosoftAccountHandler(base.get_Backchannel());
        return new CustomAcocuntHandler(Backchannel);
    }

    class CustomAcocuntHandler : OAuthHandler<CustomAccountOptions>
    {
        public CustomAcocuntHandler(HttpClient httpClient) : base(httpClient)
        {
            Console.WriteLine(">>>>>>>> CustomAcocuntHandler.ctor >>>>>>>>>");
        }
        protected async override Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
        {
            CustomLogger.Log(">>>>>>>> CustomAcocuntHandler.CreateTicketAsync Entered >>>>>>>>>");
            var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);
            //var result = await base.CreateTicketAsync(identity, properties, tokens);

            var response = await Backchannel.SendAsync(request, Context.RequestAborted);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to retrived Microsoft user information ({response.StatusCode}) Please check if the authentication information is correct and the corresponding Microsoft Account API is enabled.");
            }
            
            var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), properties, Options.AuthenticationScheme);
            var context = new OAuthCreatingTicketContext(ticket, Context, Options, Backchannel, tokens, payload);
            
            var identifier = CustomAcocuntHelper.GetId(payload);
            if (!string.IsNullOrEmpty(identifier))
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:fiservaccount:id", identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
            }
            else
            {
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, identifier, ClaimValueTypes.String, Options.ClaimsIssuer));
                identity.AddClaim(new Claim("urn:fiservaccount:id", "sai", ClaimValueTypes.String, Options.ClaimsIssuer));
            }
            
            var email = CustomAcocuntHelper.GetEmail(payload);
            if (!string.IsNullOrEmpty(email))
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, email, ClaimValueTypes.String, Options.ClaimsIssuer));
            }
            else
            {
                identity.AddClaim(new Claim(ClaimTypes.Email, "s@abc.com", ClaimValueTypes.String, Options.ClaimsIssuer));
            }
            
            CustomLogger.Log(">>>>>>>> CustomAcocuntHandler.CreateTicketAsync Creating Ticket >>>>>>>>>");
            await Options.Events.CreatingTicket(context);
            CustomLogger.Log(">>>>>>>> CustomAcocuntHandler.CreateTicketAsync Created Ticket >>>>>>>>>");
            return context.Ticket;
        }

        protected override async Task<AuthenticateResult> HandleRemoteAuthenticateAsync()
        {
            return await base.HandleRemoteAuthenticateAsync();
        }
    }

    public static class CustomAcocuntHelper
    {
        public static string GetId(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("id");
        }

        public static string GetEmail(JObject user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return user.Value<string>("email");
        }        
    }

}
