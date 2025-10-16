// BookNow.Application/Exceptions/ApplicationValidationException.cs
using System;
// Note: May need using System.Runtime.Serialization; if using serialized constructors

namespace BookNow.Application.Exceptions // <-- CRITICAL: Check this namespace
{
    public class ApplicationValidationException : Exception
    {
        // Constructor that accepts the aggregate message
        public ApplicationValidationException(string message)
            : base(message) { }

        // Optional: Default constructor
        public ApplicationValidationException()
            : base("One or more validation failures occurred.") { }

        // Optional: If you need to include the actual list of errors
        // public List<string> Errors { get; } 
        // public ApplicationValidationException(List<string> errors) : base("Validation Failed") { Errors = errors; }
    }
}