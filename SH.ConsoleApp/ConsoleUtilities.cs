using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SH.ConsoleApp.ConsoleUtilityModels;
using SH.ConsoleApp.Extensions;

namespace SH.ConsoleApp
{
  /// <summary>
  /// Provides a set of utility functions for the Console.
  /// </summary>
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

    /// <summary>
    /// Writes tabular data to the console by dividing the space available equally amongst the columns.
    /// </summary>
    /// <param name="data">The data to be written to the console. The first row is expected to be the header row.</param>
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

    /// <summary>
    /// Writes a selection menu which allows the user to select an item by using the up- and down-arrow keys and to accept the selection by pressing Enter.
    /// </summary>
    /// <param name="menuItems">The items to be available for selection.</param>
    /// <returns>Returns the index of the selected item.</returns>
    /// <example>
    /// Console.WriteLine($"How old are you?");
    /// var items = new string[5] { "0-20", "21-40", "41-60", "61-80", "81+" };
    /// var selectedIndex = ConsoleUtilities.WriteSelectionMenu(items);
    /// Console.WriteLine($"You have selected: {items[selectedIndex]}".);
    /// </example>
    public static int WriteSelectionMenu(string[] menuItems)
    {
      if (menuItems == null) throw new ArgumentNullException(nameof(menuItems));
      if (menuItems.Length == 0) throw new ArgumentException($"{nameof(menuItems)} cannot be empty");

      var menu = new SelectionMenu(menuItems);
      var selectedIndex = menu.AwaitSelection();

      return selectedIndex;
    }
  }
}
