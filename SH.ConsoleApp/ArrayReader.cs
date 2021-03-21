using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp
{
  /// <summary>
  /// Reads a forward-only stream of items from an array.
  /// </summary>
  /// <typeparam name="T">The datatype of the items in the array.</typeparam>  
  internal class ArrayReader<T>
  {
    private readonly T[] _data;
    private int _currentPosition = -1;

    /// <summary>
    /// Returns the current position.
    /// </summary>
    public int CurrentPosition => _currentPosition;

    /// <summary>
    /// Returns the item at the current position.
    /// </summary>
    public T Item => _data[_currentPosition];

    /// <summary>
    /// Initializes a new instance of <see cref="ArrayReader{T}"/>.
    /// </summary>
    /// <param name="data">The array containing the data which should be read.</param>
    public ArrayReader(T[] data)
    {
      _data = data ?? throw new ArgumentNullException(nameof(data));
    }

    /// <summary>
    /// Advances the reader to the next item in the array.
    /// </summary>
    /// <returns><see cref="true"/> if there are more items; otherwise <see cref="false"/>.</returns>
    public bool Read()
    {
      var nextPosition = _currentPosition + 1;
      if (nextPosition >= _data.Length)
      {
        return false;
      }

      _currentPosition = nextPosition;
      return true;
    }

    /// <summary>
    /// Returns the next available item but does not consume it.
    /// </summary>
    /// <returns>Returns the next available item or an empty string if there are no more items.</returns>
    /// <remarks>
    /// Technically the return value could be an empty string even if there are more items left. 
    /// This is the case when the next item in the array is actually an empty string.
    /// </remarks>
    public T Peek()
    {
      var nextPosition = _currentPosition + 1;
      if (nextPosition >= _data.Length)
      {
        return default;
      }
      else
      {
        return _data[nextPosition];
      }
    }

  }
}
