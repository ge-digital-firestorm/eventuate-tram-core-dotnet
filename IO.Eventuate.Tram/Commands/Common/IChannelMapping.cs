namespace IO.Eventuate.Tram.Commands.Common
{
	public interface IChannelMapping
	{
		string Transform(string channel);
	}
}