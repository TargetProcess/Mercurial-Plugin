using System.Runtime.Serialization;
using Tp.Integration.Plugin.Common;

[assembly: PluginAssembly("My Plugin")]

//If you rename or remove this file, it will be re-created during package update.
namespace $rootnamespace$
{
	[Profile, DataContract]
	public class Profile
	{
	}
}