using System;
using System.Collections.Generic;
using IO.Eventuate.Tram.Commands.Common;
using IO.Eventuate.Tram.Commands.Common.Paths;
using IO.Eventuate.Tram.Messaging.Common;

namespace IO.Eventuate.Tram.Commands.Consumer
{
	public class CommandHandler
	{
		// TODO: generic used to be object...
		private readonly Func<CommandMessage<object>, PathVariables, IList<IMessage>> _handler;

		public CommandHandler(string channel, string resource,
			Type commandType,
			Func<CommandMessage<TCommand>, PathVariables, List<IMessage>> handler) {
			Channel = channel;
			Resource = resource;
			CommandType = commandType;
			_handler = handler;
			// this.handler = (cm, pv) => handler.apply((CommandMessage<TCommand>) cm, pv);
		}

		public string Channel { get; }

		public Type CommandType { get; }

		public string Resource { get; }

		public bool Handles(IMessage message, ICommandTypeNamingStrategy commandTypeNamingStrategy)
		{
			return CommandTypeMatches(message, commandTypeNamingStrategy) && ResourceMatches(message);
		}

		private bool ResourceMatches(IMessage message)
		{
			if (Resource == null)
			{
				return true;
			}

			string messageResource = message.GetHeader(CommandMessageHeaders.Resource);

			return messageResource != null && ResourceMatches(messageResource, Resource);
		}

		private bool CommandTypeMatches(IMessage message, ICommandTypeNamingStrategy commandTypeNamingStrategy)
		{
			string commandTypeName = commandTypeNamingStrategy.GetCommandTypeName(CommandType);
			return commandTypeName.Equals(message.GetRequiredHeader(CommandMessageHeaders.CommandType));
		}

		private bool ResourceMatches(string messageResource, string methodPath)
		{
			ResourcePathPattern r = ResourcePathPattern.Parse(methodPath);
			ResourcePath mr = ResourcePath.Parse(messageResource);
			return r.IsSatisfiedBy(mr);
		}
		
		public IList<IMessage> InvokeMethod(CommandMessage<object> commandMessage, IDictionary<string, string> pathVars)
		{
			return _handler(commandMessage, new PathVariables(pathVars));
		}
	}
}