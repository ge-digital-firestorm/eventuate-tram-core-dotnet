using System.Collections.Generic;
using System.Threading.Tasks;
using IO.Eventuate.Tram.Commands.Common;
using IO.Eventuate.Tram.Messaging.Common;
using IO.Eventuate.Tram.Messaging.Producer;

namespace IO.Eventuate.Tram.Commands.Producer
{
	public class CommandProducer : ICommandProducer
	{
		private readonly IMessageProducer _messageProducer;
		private readonly IChannelMapping _channelMapping;
		private readonly ICommandTypeNamingStrategy _commandTypeNamingStrategy;

		public CommandProducer(IMessageProducer messageProducer, IChannelMapping channelMapping, ICommandTypeNamingStrategy commandTypeNamingStrategy)
		{
			_messageProducer = messageProducer;
			_channelMapping = channelMapping;
			_commandTypeNamingStrategy = commandTypeNamingStrategy;
		}

		public string Send(string channel, ICommand command, string replyTo, IDictionary<string, string> headers)
		{
			return Send(channel, null, command, replyTo, headers);
		}

		public string Send(string channel, string resource, ICommand command, string replyTo, IDictionary<string, string> headers)
		{
			IMessage message = MakeMessage(channel, resource, command, replyTo, headers, _commandTypeNamingStrategy);
			_messageProducer.Send(_channelMapping.Transform(channel), message);
			return message.Id;
		}

		public async Task<string> SendAsync(string channel, ICommand command, string replyTo, IDictionary<string, string> headers)
		{
			return await SendAsync(channel, null, command, replyTo, headers);
		}

		public async Task<string> SendAsync(string channel, string resource, ICommand command, string replyTo, IDictionary<string, string> headers)
		{
			IMessage message = MakeMessage(channel, resource, command, replyTo, headers, _commandTypeNamingStrategy);
			await _messageProducer.SendAsync(_channelMapping.Transform(channel), message);
			return message.Id;
		}

		public static IMessage MakeMessage(string channel, string resource, ICommand command, string replyTo, IDictionary<string, string> headers,
			ICommandTypeNamingStrategy commandTypeNamingStrategy)
		{
			string commandType = commandTypeNamingStrategy.GetCommandTypeName(command.GetType());
			MessageBuilder builder = MessageBuilder.WithPayload(JsonMapper.ToJson(command))
				.WithExtraHeaders("", headers) // TODO should these be prefixed??!
				.WithHeader(CommandMessageHeaders.Destination, channel)
				.WithHeader(CommandMessageHeaders.CommandType, commandType)
				.WithHeader(CommandMessageHeaders.ReplyTo, replyTo);

			if (resource != null)
			{
				builder.WithHeader(CommandMessageHeaders.Resource, resource);
			}

			return builder.Build();
		}
	}
}