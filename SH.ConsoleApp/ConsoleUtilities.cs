using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SH.ConsoleApp.Extensions;

namespace SH.ConsoleApp
{
  public static class ConsoleUtilities
  {
    /// <summary>
    /// Writes keys and values of a dictionary in a tabular manner.
    /// </summary>
    /// <param name="dictionary">The <see cref="Dictionary{string, string}"/> containing keys and values to write.</param>
    /// <param name="spaces">The amount of spaces to be written between keys and values.</param>
    /// <param name="indent">The amount of spaces to be written before each line. Can be used to indent the output for better formatting.</param>
    /// <remarks>Very basic for now. Does not yet implement extended functionality like linebreaks.</remarks>
    public static void WriteDictionary(Dictionary<string, string> dictionary, int spaces = 2, int indent = 0)
    {
      if (dictionary == null) throw new ArgumentNullException(nameof(dictionary));

      var longestKey = dictionary.Keys.Max(q => q.Length);
      foreach (var item in dictionary)
      {
        for (int i = 0; i < indent; i++)
        {
          Console.Write(" ");
        }
        Console.Write(item.Key.PadRight(longestKey + spaces));
        Console.Write(item.Value);
        Console.WriteLine();
      }
    }

    public static void WriteTable(string[,] data)
    {
      var columns = data.GetUpperBound(1) + 1; // +1 cause GetUpperBound is 0-based
      var rows = data.GetUpperBound(0) + 1; // +1 cause GetUpperBound is 0-based
      var columnSeparator = '|';

      // Calculate column widths:
      // Rules:
      // - For now just split the width equally amongst the columns.
      var reservedForSeparator = columns + 1;
      var columnWidth = (Console.WindowWidth - reservedForSeparator) / columns;
      var remainder = (Console.WindowWidth - reservedForSeparator) % columns;

      for (int i = 0; i < rows; i++)
      {

        for (int j = 0; j < columns; j++)
        {
          var width = columnWidth;

          // If it's the last column, extend it by the remainder:
          if (j + 1 == columns)
          {
            width += remainder;
          }

          Console.Write(columnSeparator);
          Console.Write(data[i, j].PadRightOrTruncate(width));
        }
        Console.Write(columnSeparator);
        Console.WriteLine();

        // Separate header row from data rows so it is easier to read:
        if (i == 0)
        {
          Console.WriteLine("".PadLeft(Console.WindowWidth, '-'));
        }
      }
    }
  }
}
