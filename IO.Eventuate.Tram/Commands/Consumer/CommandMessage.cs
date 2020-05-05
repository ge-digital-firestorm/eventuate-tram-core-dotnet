using System.Collections.Generic;
using IO.Eventuate.Tram.Messaging.Common;

namespace IO.Eventuate.Tram.Commands.Consumer
{
	public class CommandMessage<TCommand>
	{
		public CommandMessage(string messageId, TCommand command, IDictionary<string, string> correlationHeaders, IMessage message) {
			MessageId = messageId;
			Command = command;
			CorrelationHeaders = correlationHeaders;
			Message = message;
		}

		public IMessage Message { get; }
		
		public string MessageId { get; }
		
		public TCommand Command { get; }
		
		public IDictionary<string, string> CorrelationHeaders { get; }

		public override string ToString()
		{
			return $"{nameof(MessageId)}: {MessageId}, {nameof(Message)}: {Message}, {nameof(CorrelationHeaders)}: {CorrelationHeaders}, {nameof(Command)}: {Command}";
		}
	}
}