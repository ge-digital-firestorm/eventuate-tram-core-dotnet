using System.Diagnostics;

namespace IO.Eventuate.Tram.Commands.Common.Paths
{
	public class ResourcePath
	{
		public ResourcePath(string[] splits)
		{
			Splits = splits;
		}

		public ResourcePath(string resource)
		{
			Debug.Assert(resource.StartsWith("/"), "Should start with / " + resource);
			Splits = SplitPath(resource);
		}

		public string[] Splits { get; }

		private static string[] SplitPath(string path)
		{
			return path.Split("/");
		}

		public static ResourcePath Parse(string resource)
		{
			return new ResourcePath(resource);
		}

		public int Length => Splits.Length;

		public string ToPath()
		{
			return string.Join("/", Splits);
		}		
	}
}