using HSOne.WebApp.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace HSOne.WebApp.Extensions
{
    public static class UrlHelperExtensions
    {
        public static string ResetPasswordCallbackLink(this IUrlHelper urlHelper, string userId, string token, string scheme)
        {
            return urlHelper.Action(
                action: nameof(AuthController.ResetPassword),
                controller: "Auth",
                values: new { userId, token },
                protocol: scheme)!;
        }
    }
}
