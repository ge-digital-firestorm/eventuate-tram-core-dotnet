using System.Collections.Generic;

namespace IO.Eventuate.Tram.Commands.Common
{
	public class DefaultChannelMapping : IChannelMapping
	{
		private readonly IDictionary<string, string> _mappings;

		public class DefaultChannelMappingBuilder
		{
			private readonly IDictionary<string, string> _mappings = new Dictionary<string, string>();

			public DefaultChannelMappingBuilder With(string from, string to)
			{
				_mappings[from] = to;
				return this;
			}

			public IChannelMapping Build()
			{
				return new DefaultChannelMapping(_mappings);
			}
		}
		
		public static DefaultChannelMappingBuilder Builder()
		{
			return new DefaultChannelMappingBuilder();
		}

		public DefaultChannelMapping(IDictionary<string, string> mappings)
		{
			_mappings = mappings;
		}
		
		public string Transform(string channel)
		{
			return _mappings.TryGetValue(channel, out string value) ? value : channel;
		}
	}
}