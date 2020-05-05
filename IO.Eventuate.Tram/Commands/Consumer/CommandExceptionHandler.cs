using System;
using System.Collections.Generic;
using IO.Eventuate.Tram.Messaging.Common;

namespace IO.Eventuate.Tram.Commands.Consumer
{
	public class CommandExceptionHandler
	{
		public IList<IMessage> Invoke(Exception cause)
		{
			throw new NotImplementedException();
		}
	}
}