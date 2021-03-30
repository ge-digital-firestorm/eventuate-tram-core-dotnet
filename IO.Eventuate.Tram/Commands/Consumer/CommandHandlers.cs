using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using IO.Eventuate.Tram.Commands.Common;
using IO.Eventuate.Tram.Messaging.Common;

namespace IO.Eventuate.Tram.Commands.Consumer
{
	public class CommandHandlers
	{
		public CommandHandlers(IList<CommandHandler> handlers)
		{
			Handlers = handlers;
		}

		public ISet<string> GetChannels()
		{
			return Handlers.Select(h => h.Channel).ToImmutableHashSet();
		}

		public CommandHandler FindTargetMethod(IMessage message, ICommandTypeNamingStrategy commandTypeNamingStrategy)
		{
			return Handlers.FirstOrDefault(h => h.Handles(message, commandTypeNamingStrategy));
		}
		
		public CommandExceptionHandler FindExceptionHandler(Exception cause)
		{
			// cause.printStackTrace();
			throw new NotImplementedException("implement me", cause);
		}

		// TODO: object is probably not right
		public IList<CommandHandler> Handlers { get; }
	}
}