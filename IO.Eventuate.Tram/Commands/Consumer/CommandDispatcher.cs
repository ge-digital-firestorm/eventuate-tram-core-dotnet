using System;
using System.Collections.Generic;
using System.Linq;
using IO.Eventuate.Tram.Commands.Common;
using IO.Eventuate.Tram.Commands.Common.Paths;
using IO.Eventuate.Tram.Messaging.Common;
using IO.Eventuate.Tram.Messaging.Consumer;
using IO.Eventuate.Tram.Messaging.Producer;
using Microsoft.Extensions.Logging;

namespace IO.Eventuate.Tram.Commands.Consumer
{
	public class CommandDispatcher
	{
		private readonly ILogger _logger;

		private readonly string _commandDispatcherId;
		
		private readonly CommandHandlers _commandHandlers;

		private readonly IChannelMapping _channelMapping;

		//TODO: Needed?
		private IMessageConsumer _messageConsumer;

		private readonly IMessageProducer _messageProducer;

		public CommandDispatcher(string commandDispatcherId,
			CommandHandlers commandHandlers,
			IChannelMapping channelMapping,
			IMessageConsumer messageConsumer,
			IMessageProducer messageProducer,
			ILogger<CommandDispatcher> logger)
		{
			_commandDispatcherId = commandDispatcherId;
			_commandHandlers = commandHandlers;
			_channelMapping = channelMapping;
			_messageConsumer = messageConsumer;
			_messageProducer = messageProducer;
			_logger = logger;
		}

		// public CommandDispatcher(String commandDispatcherId, CommandHandlers commandHandlers) {
		// 	this.commandDispatcherId = commandDispatcherId;
		// 	this.commandHandlers = commandHandlers;
		// }

		// TODO: Implement CommandDispatcherInitializer
		// @PostConstruct
		// public void initialize() {
		// 	messageConsumer.subscribe(commandDispatcherId,
		// 					commandHandlers.getChannels().stream().map(channelMapping::transform).collect(toSet()),
		// 					this::messageHandler);
		// }

		public void MessageHandler(IMessage message)
		{
			// TODO Fix logging parameters in this whole file
			_logger.LogTrace("Received message {} {}", _commandDispatcherId, message);

			CommandHandler method = _commandHandlers.FindTargetMethod(message);
			if (method == null)
			{
				throw new InvalidOperationException("No method for " + message);
			}

			//CommandHandler m = possibleMethod.get();

			object param = ConvertPayload(method, message.Payload);

			IDictionary<string, string> correlationHeaders = CorrelationHeaders(message.Headers);

			IDictionary<string, string> pathVars = GetPathVars(message, method);
			
			string defaultReplyChannel = message.GetHeader(CommandMessageHeaders.ReplyTo);

			List<IMessage> replies;
			try
			{
				CommandMessage cm = new CommandMessage(message.Id, param, correlationHeaders, message);
				replies = Invoke(method, cm, pathVars);
				_logger.LogTrace("Generated replies {} {} {}", _commandDispatcherId, message, replies);
			}
			catch (Exception e)
			{
				_logger.LogTrace("Generated error {} {} {}", _commandDispatcherId, message, e.GetType().FullName);
				HandleException(message, param, method, e, pathVars, defaultReplyChannel);
				return;
			}

			if (replies != null)
			{
				Publish(correlationHeaders, replies, defaultReplyChannel);
			}
			else
			{
				_logger.LogTrace("Null replies - not publishling");
			}
		}

		protected List<Message> Invoke(CommandHandler commandHandler, CommandMessage cm, IDictionary<string, string> pathVars)
		{
			return commandHandler.InvokeMethod(cm, pathVars);
		}

		protected object ConvertPayload(CommandHandler m, String payload)
		{
			Type paramType = FindCommandParameterType(m);
			return JsonMapper.FromJson(payload, paramType);
		}

		private IDictionary<string, string> GetPathVars(IMessage message, CommandHandler handler)
		{
			string resource = handler.Resource;

			if (resource == null)
			{
				return new Dictionary<string, string>();
			}
			
			ResourcePathPattern r = ResourcePathPattern.Parse(resource);
			string messageResource = message.GetHeader(CommandMessageHeaders.Resource);

			if (messageResource == null)
			{
				return new Dictionary<string, string>();
			}
			
			ResourcePath mr = ResourcePath.Parse(messageResource);

			return r.GetPathVariableValues(mr);
		}

		private void Publish(IDictionary<string, string> correlationHeaders, IList<IMessage> replies, string defaultReplyChannel)
		{
			foreach (IMessage reply in replies)
			{
				_messageProducer.Send(_channelMapping.Transform(Destination(defaultReplyChannel)),
					MessageBuilder
						.WithMessage(reply)
						.WithExtraHeaders("", correlationHeaders)
						.Build());
			}
		}

		private string Destination(string defaultReplyChannel)
		{
			return defaultReplyChannel ?? throw new ArgumentNullException(nameof(defaultReplyChannel));
		}

		private static IDictionary<string, string> CorrelationHeaders(IDictionary<string, string> headers)
		{
			IDictionary<string, string> m = headers
				.Where(h => h.Key.StartsWith(CommandMessageHeaders.CommandHeaderPrefix)).ToDictionary(
					h => CommandMessageHeaders.InReply(h.Key),
					h => h.Value);
			
			m[ReplyMessageHeaders.InReplyTo] = headers[MessageHeaders.Id];
			
			return m;
		}

		private void HandleException(IMessage message, object param,
			CommandHandler commandHandler,
			Exception cause,
			IDictionary<string, string> pathVars,
			string defaultReplyChannel)
		{
			CommandExceptionHandler m = _commandHandlers.FindExceptionHandler(cause);

			_logger.LogInformation("Handler for {} is {}", cause.GetType(), m);

			if (m != null)
			{
				IList<IMessage> replies = m.Invoke(cause);
				Publish(CorrelationHeaders(message.Headers), replies, defaultReplyChannel);
			}
			else
			{
				IList<IMessage> replies = new List<IMessage>
					{MessageBuilder.WithPayload(JsonMapper.ToJson(new Failure())).Build()};
				Publish(CorrelationHeaders(message.Headers), replies, defaultReplyChannel);
			}
		}
		
		private static Type FindCommandParameterType(CommandHandler m) {
			return m.CommandType;
		}
	}
}