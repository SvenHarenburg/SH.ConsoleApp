using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.ConsoleUtilityModels
{
  internal class SelectionMenu
  {
    // TODO: Testing: What happens if there are more items than visible rows(Make console small)?
    // TODO: Possible feature: Multiline items. Currently items can only be as long as one line.
    // -------
    public List<SelectionMenuItem> Items { get; set; }
    private int _selectedIndex = 0;
    private bool _selectionAccepted = false;

    public SelectionMenu()
    {
      Items = new List<SelectionMenuItem>();
    }

    public SelectionMenu(string[] items)
    {
      Items = items.Select(q => new SelectionMenuItem(q)).ToList();
    }

    public int AwaitSelection()
    {
      if (Items == null || !Items.Any()) throw new ArgumentException($"{Items} cannot be null or empty.");
      InitConsoleRowIndexes();

      WriteAllItems();
      WriteInstructions();

      // Hide Cursor while selection is being made for better look and feel:
      Console.CursorVisible = false;

      // Loop until user as accepted the selection:
      while (!_selectionAccepted)
      {
        var input = Console.ReadKey(true);
        ProcessInput(input);
      }

      // The user has accepted his selection.
      // The selected item is still highlighted to redraw it without the highlighting:
      WriteItem(Items[_selectedIndex], false);

      // Move the cursor to the line after the instructions. 
      // This way the caller can go right back to writing without having to worry about the position.
      Console.SetCursorPosition(0, Items.Last().ConsoleRowIndex + 2); // +2 because 1 line is the instructions and another to get to the next empty line.

      Console.CursorVisible = true;
      return _selectedIndex;
    }

    private void ProcessInput(ConsoleKeyInfo input)
    {
      if (input == null) throw new ArgumentNullException(nameof(input));

      switch (input.Key)
      {
        case ConsoleKey.Enter:
          _selectionAccepted = true;
          break;

        case ConsoleKey.UpArrow:
          if (CanGoUp())
          {
            Update(_selectedIndex - 1);
          }
          break;

        case ConsoleKey.DownArrow:
          if (CanGoDown())
          {
            Update(_selectedIndex + 1);
          }
          break;

        default:
          break;
      }
    }

    /// <summary>
    /// Returns wether the selection can go up or not.
    /// </summary>
    /// <returns>A <see cref="Boolean"/>-value indicating if the selection can go up or not.</returns>
    private bool CanGoUp()
    {
      return _selectedIndex > 0;
    }

    /// <summary>
    /// Returns wether the selection can go down or not.
    /// </summary>
    /// <returns>A <see cref="Boolean"/>-value indicating if the selection can go down or not.</returns>
    private bool CanGoDown()
    {
      return _selectedIndex + 1 < Items.Count;
    }

    private void InitConsoleRowIndexes()
    {
      /*
       * Every item has a specific index in the Console. To easily redraw the item as selected or not-selected,
       * this index is saved in the object itself.
       * 
       * This does not yet support multiline-items.
       */

      var currentRowIndex = Console.CursorTop;
      for (int i = 0; i < Items.Count; i++)
      {
        Items[i].ConsoleRowIndex = currentRowIndex + i;
      }
    }

    /// <summary>
    /// Writes all items. Should only be called initially to write the whole menu.
    /// Afterwards <see cref="Update(int)"/> should be called to only update what has changed.
    /// </summary>
    private void WriteAllItems()
    {
      for (int i = 0; i < Items.Count; i++)
      {
        var selected = i == _selectedIndex;
        WriteItem(Items[i], selected);
      }
    }

    /// <summary>
    /// Updates the output by rewriting the previously and newly selected items.
    /// </summary>
    /// <param name="newSelectedIndex">The index of the newly selected item.</param>
    private void Update(int newSelectedIndex)
    {
      if (newSelectedIndex < 0) throw new ArgumentOutOfRangeException(nameof(newSelectedIndex), $"{nameof(newSelectedIndex)} cannot be less than zero.");
      if (newSelectedIndex >= Items.Count) throw new ArgumentOutOfRangeException(nameof(newSelectedIndex), $"There are only {Items.Count} items. The zero-based selectedIndex cannot be higher than {Items.Count - 1}");

      // Write previously selected item as normal:      
      var previouslySelectedItem = Items[_selectedIndex];
      WriteItem(previouslySelectedItem, false);

      _selectedIndex = newSelectedIndex;

      // Write new selected item as selected:
      var newSelectedItem = Items[_selectedIndex];
      WriteItem(newSelectedItem, true);

    }

    private void WriteItem(SelectionMenuItem item, bool selected)
    {
      if (item == null) throw new ArgumentNullException(nameof(item));
      var indexOfItem = Items.IndexOf(item);
      // Clear line:
      Console.SetCursorPosition(0, item.ConsoleRowIndex);
      Console.Write("".PadRight(Console.WindowWidth));

      if (selected)
      {
        // TODO: Make colors configurable.
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
      }

      // Write item:
      Console.SetCursorPosition(0, item.ConsoleRowIndex);
      Console.WriteLine($"{indexOfItem + 1}. {item.Text}");

      Console.ResetColor();
    }

    private void WriteInstructions()
    {
      Console.WriteLine($"Please select an option by moving up(↑) or down(↓). Press Enter to accept.");
    }
  }
}
