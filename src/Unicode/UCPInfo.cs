// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// UCPInfo.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TypeHelp
{
   public readonly struct UCPInfo // UniCode Point Info
   {
      public readonly CPG  Group;
      public readonly int  Index;
      public readonly bool Strict;
      public readonly bool Valid;
      public readonly int  Value;

      public bool Is(CPG group)
      {
         return Group == group;
      }

      public UCPInfo(bool strict, int  value)
      {
         CPG group = Group = UCSurrogator.CPGroupOf(strict, value);
         Strict = strict;
         Index = -1;
         Valid = group >= UCSurrogator.MinValidCPGroup;
         Value = value;
      }

      public UCPInfo(bool strict, int value, int index)
      {
         CPG group = Group = UCSurrogator.CPGroupOf(strict, value);
         Strict = strict;
         Index = index;
         Valid = group >= UCSurrogator.MinValidCPGroup;
         Value = value;
      }

      public UCPInfo(bool strict, char value)
      {
         CPG group = Group = UCSurrogator.CPGroupOf(strict, (int)value);
         Strict = strict;
         Index = -1;
         Valid = group >= UCSurrogator.MinValidCPGroup;
         Value = value;
      }

      public UCPInfo(bool strict, char value, int index)
      {
         CPG group = Group = UCSurrogator.CPGroupOf(strict, (int)value);
         Strict = strict;
         Index = index;
         Valid = group >= UCSurrogator.MinValidCPGroup;
         Value = (int)value;
      }

      public UCPInfo(bool strict, string value, int index)
      {
         int intVal = (index > -1 && null != value &&
            index < value.Length)
               ? value[index]
               : UCSurrogator.ReplacementInt;

         CPG group = Group = UCSurrogator.CPGroupOf(strict, intVal);
         Strict = strict;
         Index = index;
         Valid = group >= UCSurrogator.MinValidCPGroup;
         Value = intVal;
      }
   }
}
