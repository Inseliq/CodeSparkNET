namespace CodeSparkNET.Domain.Models
{
    /// <summary>
    /// Represents the error details displayed in the error view.
    /// Contains information about the request identifier,
    /// which can be useful for debugging and tracing issues.
    /// </summary>
    public class ErrorViewModel
    {
        /// <summary>
        /// Unique identifier of the request associated with the error.
        /// Useful for tracking and correlating logs.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="RequestId"/> should be displayed.
        /// Returns true if the RequestId is not null or empty.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
