using System.Collections.Generic;

namespace IO.Eventuate.Tram.Commands.Consumer
{
	public class PathVariables
	{
		private readonly IDictionary<string, string> _pathVars;

		public PathVariables(IDictionary<string, string> pathVars)
		{
			_pathVars = pathVars;
		}

		public string GetString(string name) {
			_pathVars.TryGetValue(name, out string value);
			return value;
		}

		public long GetLong(string name) {
			return long.Parse(GetString(name));
		}
	}
}