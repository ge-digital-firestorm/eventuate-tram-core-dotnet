using System.Collections.Generic;
using System.Threading.Tasks;
using IO.Eventuate.Tram.Commands.Common;

namespace IO.Eventuate.Tram.Commands.Producer
{
	public interface ICommandProducer
	{
		/// <summary>
		/// Sends a command
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="command">The command to send</param>
		/// <param name="replyTo"></param>
		/// <param name="headers">Additional headers</param>
		/// <returns>The ID of the sent command</returns>
		string Send(string channel, ICommand command, string replyTo, IDictionary<string, string> headers);

		/// <summary>
		/// Sends a command
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="resource"></param>
		/// <param name="command">The command to send</param>
		/// <param name="replyTo"></param>
		/// <param name="headers">Additional headers</param>
		/// <returns>The ID of the sent command</returns>
		string Send(string channel, string resource, ICommand command, string replyTo, IDictionary<string, string> headers);		
		
		/// <summary>
		/// Sends a command
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="command">The command to send</param>
		/// <param name="replyTo"></param>
		/// <param name="headers">Additional headers</param>
		/// <returns>The ID of the sent command</returns>
		Task<string> SendAsync(string channel, ICommand command, string replyTo, IDictionary<string, string> headers);

		/// <summary>
		/// Sends a command
		/// </summary>
		/// <param name="channel"></param>
		/// <param name="resource"></param>
		/// <param name="command">The command to send</param>
		/// <param name="replyTo"></param>
		/// <param name="headers">Additional headers</param>
		/// <returns>The ID of the sent command</returns>
		Task<string> SendAsync(string channel, string resource, ICommand command, string replyTo, IDictionary<string, string> headers);
	}
}