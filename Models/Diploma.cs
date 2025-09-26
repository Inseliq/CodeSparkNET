namespace CodeSparkNET.Models
{
    /// <summary>
    /// Represents a diploma product that can be issued to users.
    /// Inherits base product properties and adds information 
    /// about the issuing organization.
    /// </summary>
    public class Diploma : Product
    {
        /// <summary>
        /// The organization or institution that issued the diploma.
        /// </summary>
        public string? Issuer { get; set; }
    }
}
