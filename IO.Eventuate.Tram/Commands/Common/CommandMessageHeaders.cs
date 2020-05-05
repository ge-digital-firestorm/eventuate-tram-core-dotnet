using System.Diagnostics;

namespace IO.Eventuate.Tram.Commands.Common
{
	public static class CommandMessageHeaders
	{
		public const string CommandHeaderPrefix = "command_";
		public const string CommandType = CommandHeaderPrefix + "type";
		public const string Resource = CommandHeaderPrefix + "resource";
		public const string Destination = CommandHeaderPrefix + "_destination";
		
		public const string CommandReplyPrefix = "commandreply_";
		public const string ReplyTo = CommandReplyPrefix + "reply_to";

		public static string InReply(string header)
		{
			Debug.Assert(header.StartsWith(CommandHeaderPrefix));

			return CommandReplyPrefix + header.Substring(CommandHeaderPrefix.Length);
		}
	}
}