using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SH.ConsoleApp.ConsoleUtilityModels
{
  internal class SelectionMenuItem
  {
    public string Text { get; set; }
    public int ConsoleRowIndex { get; set; }
    public SelectionMenuItem(string text)
    {
      Text = text;
    }
  }
}
