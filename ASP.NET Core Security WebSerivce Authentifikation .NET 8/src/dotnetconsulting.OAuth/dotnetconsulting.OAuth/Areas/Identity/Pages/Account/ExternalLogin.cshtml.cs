// Disclaimer
// Dieser Quellcode ist als Vorlage oder als Ideengeber gedacht. Er kann frei und ohne 
// Auflagen oder Einschränkungen verwendet oder verändert werden.
// Jedoch wird keine Garantie übernommen, dass eine Funktionsfähigkeit mit aktuellen und 
// zukünftigen API-Versionen besteht. Der Autor übernimmt daher keine direkte oder indirekte 
// Verantwortung, wenn dieser Code gar nicht oder nur fehlerhaft ausgeführt wird.
// Für Anregungen und Fragen stehe ich jedoch gerne zur Verfügung.

// Thorsten Kansy, www.dotnetconsulting.eu

#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace dotnetconsulting.OAuth.Areas.Identity.Pages.Account;

[AllowAnonymous]
public class ExternalLoginModel(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager,
    ILogger<ExternalLoginModel> logger,
    IEmailSender emailSender) : PageModel
{
    [BindProperty]
    public InputModel Input { get; set; }

    public string ProviderDisplayName { get; set; }

    public string ReturnUrl { get; set; }

    [TempData]
    public string ErrorMessage { get; set; }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

    public IActionResult OnGetAsync()
    {
        return RedirectToPage("./Login");
    }

    public IActionResult OnPost(string provider, string returnUrl = null)
    {
        // Request a redirect to the external login provider.
        var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
        var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
    {
        returnUrl ??= Url.Content("~/");
        if (remoteError != null)
        {
            ErrorMessage = $"Error from external provider: {remoteError}";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        // Sign in the user with this external login provider if the user already has a login.
        var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
        if (result.Succeeded)
        {
            logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
            return LocalRedirect(returnUrl);
        }
        if (result.IsLockedOut)
        {
            return RedirectToPage("./Lockout");
        }
        else
        {
            // If the user does not have an account, then ask the user to create an account.
            ReturnUrl = returnUrl;
            ProviderDisplayName = info.ProviderDisplayName;
            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
            {
                Input = new InputModel
                {
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                };
            }
            return Page();
        }
    }

    public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        // Get the information about the user from the external login provider
        var info = await signInManager.GetExternalLoginInfoAsync();
        if (info == null)
        {
            ErrorMessage = "Error loading external login information during confirmation.";
            return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
        }

        if (ModelState.IsValid)
        {
            var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

            var result = await userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await userManager.AddLoginAsync(user, info);
                if (result.Succeeded)
                {
                    switch (info.LoginProvider)
                    {
                        case "Microsoft":
                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.GivenName));
                            }

                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.Surname));
                            }

                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.Name));
                            }
                            break;
                        case "LinkedIn":
                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.GivenName));
                            }

                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.Surname));
                            }

                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.Name));
                            }

                            if (info.Principal.HasClaim(c => c.Type == "urn:linkedin:pictureurl"))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst("urn:linkedin:pictureurl"));
                            }

                            if (info.Principal.HasClaim(c => c.Type == "urn:linkedin:pictureurls"))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst("urn:linkedin:pictureurls"));
                            }

                            break;
                        case "Google":
                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.GivenName));
                            }

                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Surname))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.Surname));
                            }

                            if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst(ClaimTypes.Name));
                            }

                            if (info.Principal.HasClaim(c => c.Type == "urn:google:locale"))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst("urn:google:locale"));
                            }

                            if (info.Principal.HasClaim(c => c.Type == "urn:google:picture"))
                            {
                                await userManager.AddClaimAsync(user,
                                    info.Principal.FindFirst("urn:google:picture"));
                            }
                            break;
                    }


                    logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);

                    var userId = await userManager.GetUserIdAsync(user);
                    var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId, code },
                        protocol: Request.Scheme);

                    await emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // If account confirmation is required, we need to show the link if we don't have a real email sender
                    if (userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("./RegisterConfirmation", new { Input.Email });
                    }

                    await signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);

                    return LocalRedirect(returnUrl);
                }
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        ProviderDisplayName = info.ProviderDisplayName;
        ReturnUrl = returnUrl;
        return Page();
    }
}