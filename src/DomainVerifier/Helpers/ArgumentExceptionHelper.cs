using System;

namespace DomainVerifier.Helpers
{
    public static class ArgumentExceptionHelper
    {
        public static void ThrowIfNullOrEmpty(string? value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The parameter '{paramName}' cannot be null or empty.");
            }
        }
        
        public static void ThrowIfNotLongEnough(int actualLength, int minimumLength, string paramName)
        {
            if (actualLength < minimumLength)
            {
                throw new ArgumentException($"The parameter '{paramName}' cannot be less than {minimumLength}.");
            }
        }
    }
}