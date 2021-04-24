using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.Extensions
{
  public static class StructExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks>Thanks to https://stackoverflow.com/a/5635729 </remarks>
    public static bool IsDefault<T>(this T value) where T : struct
    {
      return value.Equals(default(T));
    }
  }
}
