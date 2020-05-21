using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Settings;
using Passingwind.Blog.Services;
using Passingwind.Blog.Web.Models.Account;
using Passingwind.Blog.Web.Services;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Controllers
{
	[Authorize]
	public class AccountController : BlogControllerBase
	{
		const string ControllerName = "Account";

		private readonly ILogger<AccountController> _logger;
		private readonly BlogUserManager _userManager;
		private readonly BlogSignInManager _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly BlogOptions _blogOptions;

		private readonly AdvancedSettings _advancedSettings;

		public StatusMessageViewModel StatusMessage
		{
			get { return string.IsNullOrWhiteSpace((string)TempData["StatusMessage"]) ? null : JsonConvert.DeserializeObject<StatusMessageViewModel>((string)TempData["StatusMessage"]); }
			set { TempData["StatusMessage"] = JsonConvert.SerializeObject(value); }
		}

		public AccountController(ILogger<AccountController> logger, BlogUserManager blogUserManager, BlogSignInManager signInManager, IEmailSender emailSender, IOptionsSnapshot<BlogOptions> blogOptions, AdvancedSettings advancedSettings)
		{
			_logger = logger;
			_userManager = blogUserManager;
			_signInManager = signInManager;
			_emailSender = emailSender;

			_blogOptions = blogOptions.Value;
			_advancedSettings = advancedSettings;
		}

		protected async Task SendEmailConfirmationMessageAsync(User user)
		{
			var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Action("ConfirmEmail", ControllerName, new { area = "", userId = user.Id, code = code }, protocol: Request.Scheme);

			// TODO 
			await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
		}

		protected async Task SendEmailResetPasswordMessageAsync(User user)
		{
			var code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
			var callbackUrl = Url.Action("ResetPassword", ControllerName, new { area = "", userId = user.Id, code }, protocol: Request.Scheme);

			await _emailSender.SendEmailAsync(
				user.Email,
				"Reset Password",
				$"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
		}

		#region Login/Logout

		[Route("/account/login", Name = "login")]
		[AllowAnonymous]
		public async Task<IActionResult> LoginAsync(string returnUrl)
		{
			ViewData["ReturnUrl"] = returnUrl ?? Url.Content("~/");

			// Clear the existing external cookie to ensure a clean login process
			await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

			return View();
		}

		[AllowAnonymous]
		[HttpPost(Name = "login")]
		public async Task<IActionResult> LoginAsync(LoginViewModel model, string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");

			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null)
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}

				var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: _blogOptions.Account.LockoutOnFailure);
				if (result.Succeeded)
				{
					_logger.LogInformation("User logged in.");
					return LocalRedirect(returnUrl);
				}

				if (result.RequiresTwoFactor)
				{
					// TODO 
					return RedirectToAction("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
				}
				else if (result.IsLockedOut)
				{
					_logger.LogWarning("User account locked out.");
					return RedirectToAction("Lockout");
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[Route("/account/logout", Name = "logout")]
		public async Task<IActionResult> LogoutAsync()
		{
			await _signInManager.SignOutAsync();

			return LocalRedirect("~/");
		}

		#endregion

		#region Register

		[Route("/account/register", Name = "register")]
		[AllowAnonymous]
		public IActionResult Register()
		{
			if (!_advancedSettings.EnableRegister)
				return NotFound();

			return View();
		}

		[AllowAnonymous]
		[HttpPost(Name = "register")]
		public async Task<IActionResult> RegisterAsync(RegisterViewModel model, string returnUrl = null)
		{
			if (!_advancedSettings.EnableRegister)
				return NotFound();

			returnUrl = returnUrl ?? Url.Content("~/");

			if (ModelState.IsValid)
			{
				var user = new Blog.Data.Domains.User
				{
					UserName = model.Email,
					Email = model.Email,
					DisplayName = model.Email,
				};
				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						await SendEmailConfirmationMessageAsync(user);

						return RedirectToAction("RegisterConfirmation", new { email = model.Email });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);

						return LocalRedirect(returnUrl);
					}
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			// If we got this far, something failed, redisplay form
			return View(model);
		}

		[AllowAnonymous]
		public async Task<IActionResult> RegisterConfirmationAsync(string email)
		{
			if (email == null)
			{
				return BadRequest();
			}

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return NotFound($"Unable to load user with email '{email}'.");
			}

			var model = new RegisterConfirmationViewModel()
			{
				Email = email,
			};

			return View(model);
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> SendEmailConfirmationMessageAsync(string email)
		{
			if (email == null)
			{
				return BadRequest();
			}

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return NotFound($"Unable to load user with email '{email}'.");
			}

			if (await _userManager.IsEmailConfirmedAsync(user))
			{
				_logger.LogWarning($"The email '{email}' is confirmed.");
				return Ok();
			}

			await SendEmailConfirmationMessageAsync(user);

			return Ok();
		}

		#endregion

		#region Lockout

		[AllowAnonymous]
		public IActionResult Lockout()
		{
			return View();
		}

		#endregion

		#region ChangePassword

		[HttpGet(Name = "changepassword")]
		public async Task<IActionResult> ChangePasswordAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);
			if (!hasPassword)
			{
				return RedirectToAction("SetPassword");
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
			if (!changePasswordResult.Succeeded)
			{
				foreach (var error in changePasswordResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View();
			}

			await _signInManager.RefreshSignInAsync(user);
			_logger.LogInformation("User changed their password successfully.");

			StatusMessage = StatusMessageViewModel.Succeed("Your password has been changed.");

			return View();
		}

		public async Task<IActionResult> SetPasswordAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var hasPassword = await _userManager.HasPasswordAsync(user);

			if (hasPassword)
			{
				return RedirectToAction("ChangePassword");
			}

			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SetPasswordAsync(SetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			var addPasswordResult = await _userManager.AddPasswordAsync(user, model.NewPassword);
			if (!addPasswordResult.Succeeded)
			{
				foreach (var error in addPasswordResult.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
				return View();
			}

			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = StatusMessageViewModel.Succeed("Your password has been set.");

			return View();
		}

		#endregion

		#region ResetPassword

		[AllowAnonymous]
		public async Task<IActionResult> ResetPasswordAsync(string userId, string code)
		{
			if (code == null || userId == null)
			{
				return BadRequest("A code must be supplied for password reset.");
			}

			var user = await _userManager.FindByIdAsync(userId);

			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userId}'.");
			}

			var model = new ResetPasswordViewModel()
			{
				Code = code,
				Email = user.Email,
			};

			return View(model);
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var user = await _userManager.FindByEmailAsync(model.Email);
			if (user == null)
			{
				// Don't reveal that the user does not exist
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}

			string code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(model.Code));

			var result = await _userManager.ResetPasswordAsync(user, code, model.Password);

			if (result.Succeeded)
			{
				return RedirectToAction(nameof(ResetPasswordConfirmation));
			}

			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return View(model);
		}

		[AllowAnonymous]
		public IActionResult ResetPasswordConfirmation()
		{
			return View();
		}

		#endregion

		#region ConfirmEmail

		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmailAsync(string userId, string code)
		{
			if (userId == null || code == null)
			{
				return BadRequest();
			}

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{userId}'.");
			}

			code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
			var result = await _userManager.ConfirmEmailAsync(user, code);

			StatusMessage = new StatusMessageViewModel(result.Succeeded, result.Succeeded ? "Thank you for confirming your email." : "Error confirming your email.");

			return View();
		}

		#endregion

		#region ExternalLogin

		[AllowAnonymous]
		[HttpPost]
		public IActionResult ExternalLogin(string provider, string returnUrl = null)
		{
			// Request a redirect to the external login provider.
			var redirectUrl = Url.Action("ExternalLoginCallback", new { returnUrl });
			var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
			return new ChallengeResult(provider, properties);

		}

		[AllowAnonymous]
		public async Task<IActionResult> ExternalLoginCallbackAsync(string returnUrl = null, string remoteError = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			if (remoteError != null)
			{
				StatusMessage = StatusMessageViewModel.Error($"Error from external provider: {remoteError}");
				return RedirectToAction("Login", new { ReturnUrl = returnUrl });
			}

			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				StatusMessage = StatusMessageViewModel.Error("Error loading external login information.");
				return RedirectToAction("Login", new { ReturnUrl = returnUrl });
			}

			// Sign in the user with this external login provider if the user already has a login.
			var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
			if (result.Succeeded)
			{
				_logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
				return LocalRedirect(returnUrl);
			}

			if (result.IsLockedOut)
			{
				return RedirectToAction("Lockout");
			}
			else
			{
				// If the user does not have an account, then ask the user to create an account.
				var confirmationModel = new ExternalLoginConfirmationViewModel()
				{
					ReturnUrl = returnUrl,
					LoginProvider = info.LoginProvider,
				};

				if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
				{
					confirmationModel.Email = info.Principal.FindFirstValue(ClaimTypes.Email);
				}

				return View("ExternalLoginConfirmation", confirmationModel);
			}

		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> ExternalLoginConfirmationAsync(ExternalLoginConfirmationViewModel model, string returnUrl)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			// Get the information about the user from the external login provider
			var info = await _signInManager.GetExternalLoginInfoAsync();
			if (info == null)
			{
				StatusMessage = StatusMessageViewModel.Error("Error loading external login information during confirmation.");
				return RedirectToAction("Login", new { ReturnUrl = returnUrl });
			}

			IdentityResult result = IdentityResult.Success;
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
				if (user == null)
				{
					user = new User { UserName = model.Email, Email = model.Email, DisplayName = model.Email, };

					if (info.Principal.HasClaim(t => t.Type == ClaimTypes.Name))
					{
						user.DisplayName = info.Principal.FindFirstValue(ClaimTypes.Name);
					}
					if (info.Principal.HasClaim(t => t.Type == ClaimTypes.GivenName))
					{
						user.DisplayName = info.Principal.FindFirstValue(ClaimTypes.GivenName);
					}
					if (info.Principal.HasClaim(t => t.Type == ClaimTypes.MobilePhone))
					{
						user.PhoneNumber = info.Principal.FindFirstValue(ClaimTypes.MobilePhone);
					}

					// TODO save claims

					result = await _userManager.CreateAsync(user);

					if (result.Succeeded)
					{
						result = await _userManager.AddLoginAsync(user, info);
						if (result.Succeeded)
						{
							_logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
						}
					}
				}

				if (result.Succeeded)
				{
					if (!(await _userManager.IsEmailConfirmedAsync(user)) && _userManager.Options.SignIn.RequireConfirmedAccount)
					{
						await SendEmailConfirmationMessageAsync(user);

						return RedirectToAction("RegisterConfirmation", new { email = model.Email });
					}
					else
					{
						await _signInManager.SignInAsync(user, isPersistent: false);

						return LocalRedirect(returnUrl);
					}
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return View(model);
		}

		#endregion

		#region ForgotPassword

		[AllowAnonymous]
		[HttpGet(Name = "forgotpassword")]
		public IActionResult ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
				{
					// Don't reveal that the user does not exist or is not confirmed
					return RedirectToAction(nameof(ForgotPasswordConfirmation));
				}

				// For more information on how to enable account confirmation and password reset please 
				// visit https://go.microsoft.com/fwlink/?LinkID=532713
				await SendEmailResetPasswordMessageAsync(user);

				return RedirectToAction(nameof(ForgotPasswordConfirmation));
			}

			return View(model);

		}

		public IActionResult ForgotPasswordConfirmation()
		{
			return View();
		}

		#endregion
	}
}
