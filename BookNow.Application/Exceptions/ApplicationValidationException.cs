using System;

namespace BookNow.Application.Exceptions 
{
    public class ApplicationValidationException : Exception
    {
      
        public ApplicationValidationException(string message)
            : base(message) { }

        
        public ApplicationValidationException()
            : base("One or more validation failures occurred.") { }

    }
}