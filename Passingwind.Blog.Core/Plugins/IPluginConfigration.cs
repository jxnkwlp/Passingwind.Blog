using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins
{
	public interface IPluginConfigure
	{
		void GetConfigureRouteData(out string controller, out string action);
	}

	public interface IPluginConfigration<TConfigModel> : IPluginConfigure where TConfigModel : PluginConfigurationModel
	{
		Task<TConfigModel> GetConfigDataAsync();

		Task PostConfigDataAsync(TConfigModel model);
	}

	public class PluginConfigurationModel { }
}
