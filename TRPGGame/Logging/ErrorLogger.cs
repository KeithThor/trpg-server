using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace TRPGGame.Logging
{
    /// <summary>
    /// Class responsible for logging errors to an error log file.
    /// </summary>
    public class ErrorLogger : IErrorLogger
    {
        /// <summary>
        /// Logs an error message to the error log file asynchronously.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <returns>Returns a task object for the asynchronous operation.</returns>
        public Task LogAsync(string message)
        {
            using (var fileStream = File.OpenWrite("error-log.txt"))
            {
                byte[] messageAsBytes = new UTF8Encoding(true).GetBytes(message);
                return fileStream.WriteAsync(messageAsBytes, 0, messageAsBytes.Length);
            }
        }

        /// <summary>
        /// Logs an error message to the error log file.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public void Log(string message)
        {
            using (var fileStream = File.OpenWrite("error-log.txt"))
            {
                byte[] messageAsBytes = new UTF8Encoding(true).GetBytes(message);
                fileStream.Write(messageAsBytes, 0, messageAsBytes.Length);
            }
        }
    }
}
