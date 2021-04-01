using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Extensions
{
  /// <summary>
  /// Contains extensions for <see cref="string"/>.
  /// </summary>
  internal static class StringExtensions
  {

    /// <summary>
    /// Returns a new string which is either padded with spaces up to a specified length or is truncated to that length.
    /// Optionally removes characters at the end of the string and replaces them with dots to indicate that the string has been cut off.
    /// </summary>    
    /// <param name="length">The target length of the string.</param>
    /// <param name="numberOfDots">The number of characters that are cut off and replaced with dots at the end of the string if the string is longer than the intended length.
    /// Can be zero but not less than zero.</param>
    /// <returns></returns>    
    internal static string PadRightOrTruncate(this string value, int length, int numberOfDots = 2)
    {
      // Throw exception if less than zero because that would mess up the math:
      if (numberOfDots < 0) throw new ArgumentOutOfRangeException($"{numberOfDots} cannot be less than zero.");

      // length does not have to be checked for less than zero because the PadRight-Function already does that.

      if (value.Length == length)
      {
        return value;
      }

      if (value.Length < length)
      {
        return value.PadRight(length);
      }

      if (value.Length > length)
      {
        value = value.Substring(0, length - numberOfDots);
        value = value.PadRight(length, '.');
      }

      return value;
    }
  }
}
