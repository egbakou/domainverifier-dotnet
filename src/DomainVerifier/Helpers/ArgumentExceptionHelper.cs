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
    }
}