using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(JoinGo.App_Start.Startup))]
namespace JoinGo.App_Start
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCookieAuthentication(new CookieAuthenticationOptions
			{
				AuthenticationType = "ApplicationCookie",
				LoginPath = new PathString("/Home/Login")
			});

			app.UseExternalSignInCookie("ExternalCookie");

			app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
			{
				ClientId = "237604062463-3b3iu9o8cli6vgporbaj2d4cphuviu70.apps.googleusercontent.com",
				ClientSecret = "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",

				CallbackPath = new PathString("/Oauths/callback"),
				Scope = { "openid", "email", "profile" }

			});
		}

	}
}