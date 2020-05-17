using NUnit.Framework;

namespace Passingwind.Blog.Services.Impl.Tests
{
	[TestFixture()]
	public class SlugServicesTests
	{
		[Test()]
		public void GetSeNameTest_Letter()
		{
			var result = SlugServices.GetSeName("this is title...", true, false);
			Assert.AreEqual(result, "this-is-title");
		}

		[Test()]
		public void GetSeNameTest_Digit()
		{
			var result = SlugServices.GetSeName("this is title with number 123456...", true, false);
			Assert.AreEqual(result, "this-is-title-with-number-123456");
		}

		[Test()]
		public void GetSeNameTest_UnicodeChar()
		{
			var result = SlugServices.GetSeName("this is title with 标题？", true, false);
			Assert.AreEqual(result, "this-is-title-with-");

			result = SlugServices.GetSeName("this is title with 标题？", true, true);
			Assert.AreEqual(result, "this-is-title-with-标题");
		}

		[Test()]
		public void GetSeNameTest_SpecialCharacters()
		{
			var result = SlugServices.GetSeName("this is title with &%$-=-+-fdsfd", true, false);
			Assert.AreEqual(result, "this-is-title-with-fdsfd");
		}
	}
}
