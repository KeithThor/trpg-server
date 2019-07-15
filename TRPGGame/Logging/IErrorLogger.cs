using System.Threading.Tasks;

namespace TRPGGame.Logging
{
    /// <summary>
    /// Interface with functions for logging error messages.
    /// </summary>
    public interface IErrorLogger
    {
        /// <summary>
        /// Logs an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        void Log(string message);

        /// <summary>
        /// Logs an error message asynchronously.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <returns>Returns a task object for the asynchronous operation.</returns>
        Task LogAsync(string message);
    }
}