using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Tests
{
  // TODO: Do research and evaluate if the ArrayReader needs more complex tests since it accepts a generic parameter. The tests currently test with either int or int? and I am not sure if that is enough.

  [TestFixture]
  public class ArrayReaderTests
  {
    [TestFixture]
    public class ConstructorTests
    {
      [Test]
      public void ThrowsArgumentNullWhenDataIsNull()
      {
        Assert.Throws<ArgumentNullException>(() => new ArrayReader<object>(null));
      }
    }

    [TestFixture]
    public class ReadTests
    {
      private int[] _data;
      private ArrayReader<int> _arrayReader;

      [SetUp]
      public void Setup()
      {
        _data = new int[] { 100, 200, 300, 400 };
        _arrayReader = new ArrayReader<int>(_data);
      }

      [Test]
      public void TrueIfThereAreMoreItems()
      {
        for (int i = 0; i < _data.Length; i++)
        {
          var result = _arrayReader.Read();
          Assert.True(result);
        }
      }

      [Test]
      public void FalseIfThereAreNoMoreItems()
      {
        // Read all of the data:
        for (int i = 0; i < _data.Length; i++)
        {
          _arrayReader.Read();
        }

        // Now read once more after everything has been read:
        var result = _arrayReader.Read();
        Assert.False(result);
      }

      [Test]
      public void AdvancesCurrentPosition()
      {
        for (int i = 0; i < _data.Length; i++)
        {
          _arrayReader.Read();
          Assert.AreEqual(i, _arrayReader.CurrentPosition);
        }
      }
    }

    [TestFixture]
    public class PeekTests
    {
      private int[] _data;
      private ArrayReader<int> _arrayReader;

      [SetUp]
      public void Setup()
      {
        _data = new int[] { 100, 200, 300, 400 };
        _arrayReader = new ArrayReader<int>(_data);
      }

      [Test]
      public void ReturnsCorrectItem()
      {
        // Starting at -1 because that's the initial position of the reader when nothing has been read yet.
        for (int i = -1; i < _data.Length; i++)
        {
          // If there are items after the current one:
          if (i + 1 < _data.Length)
          {
            Assert.AreEqual(_data[i + 1], _arrayReader.Peek());
            _arrayReader.Read();
          }
        }
      }

      [Test]
      public void ReturnsDefaultWhenNoMoreItemsOnNonNullableType()
      {
        // Read to the end:
        while (_arrayReader.Read())
        {
        }
        Assert.AreEqual(default(int), _arrayReader.Peek());
      }

      [Test]
      public void ReturnsNullWhenNoMoreItemsOnNullableType()
      {
        // This test needs an array with nullable type so it
        // initializes its own instance:
        var nullableData = new int?[] { 1, 2, 3, 4 };
        var nullableArrayReader = new ArrayReader<int?>(nullableData);

        // Read to the end:
        while (nullableArrayReader.Read())
        {
        }
        Assert.AreEqual(null, nullableArrayReader.Peek());
      }


      [Test]
      public void DoesNotAdvanceCurrentPosition()
      {
        for (int i = 0; i < _data.Length; i++)
        {
          _arrayReader.Read();
          _arrayReader.Peek();
          Assert.AreEqual(i, _arrayReader.CurrentPosition);
        }
      }
    }

    [TestFixture]
    public class ItemTests
    {
      private int[] _data;
      private ArrayReader<int> _arrayReader;

      [SetUp]
      public void Setup()
      {
        _data = new int[] { 100, 200, 300, 400 };
        _arrayReader = new ArrayReader<int>(_data);
      }

      [Test]
      public void ReturnsCorrectItem()
      {
        for (int i = 0; i < _data.Length; i++)
        {
          _arrayReader.Read();

          Assert.AreEqual(_data[i], _arrayReader.Item);
        }
      }
    }
  }
}
