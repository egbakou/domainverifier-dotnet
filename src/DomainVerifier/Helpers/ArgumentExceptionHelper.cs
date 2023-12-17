using System;

namespace DomainVerifier.Helpers
{
    /// <summary>
    /// Argument exception helper.
    /// </summary>
    public static class ArgumentExceptionHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the value is null or empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="ArgumentException">The parameter cannot be null or empty.</exception>
        public static void ThrowIfNullOrEmpty(string? value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException($"The parameter '{paramName}' cannot be null or empty.");
            }
        }
        
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the value is less than the minimum length.
        /// </summary>
        /// <param name="actualLength">The actual length.</param>
        /// <param name="minimumLength">The minimum length.</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="ArgumentException">The parameter cannot be less than the minimum length.</exception>
        public static void ThrowIfNotLongEnough(int actualLength, int minimumLength, string paramName)
        {
            if (actualLength < minimumLength)
            {
                throw new ArgumentException($"The parameter '{paramName}' cannot be less than {minimumLength}.");
            }
        }
    }
}