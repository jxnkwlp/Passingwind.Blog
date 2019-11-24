namespace Passingwind.Blog.Web.Captcha
{
	public interface ICaptchaService
	{
		CaptchaImageData GenerateCaptchaImage(string id, int length = 4);

		bool Validate(string id, string input);
	}
}