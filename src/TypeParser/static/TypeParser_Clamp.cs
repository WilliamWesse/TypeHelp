// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// TypeParser_Clamp.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Numerics;

namespace TypeHelp
{
   public partial class TypeParser
   {
      #region public static Clamp methods

      public static char Clamp(char value, char max)
      {
         return (char)TypeParser.Clamp(
            (ulong)value, ulong.MinValue, (ulong)max);
      }

      public static char Clamp(char value, char min, char max)
      {
         return (char)TypeParser.Clamp((ulong)value, (ulong)min, (ulong)max);
      }

      public static sbyte Clamp(sbyte value, sbyte max)
      {
         return (sbyte)TypeParser.Clamp(false, (long)value, 0, (long)max);
      }

      public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
      {
         return (sbyte)TypeParser.Clamp(
            false, (long)value, (long)min, (long)max);
      }

      public static sbyte Clamp(bool abs, sbyte value, sbyte min, sbyte max)
      {
         return (sbyte)TypeParser.Clamp(
            abs, (long)value, (long)min, (long)max);
      }

      public static byte Clamp(byte value, byte max)
      {
         return (byte)TypeParser.Clamp(
            (ulong)value, ulong.MinValue, (ulong)max);
      }

      public static byte Clamp(byte value, byte min, byte max)
      {
         return (byte)TypeParser.Clamp((ulong)value, (ulong)min, (ulong)max);
      }

      public static short Clamp(short value, short max)
      {
         return (short)TypeParser.Clamp(
            false, (long)value, (long)0, (long)max);
      }

      public static short Clamp(short value, short min, short max)
      {
         return (short)TypeParser.Clamp(
            false, (long)value, (long)min, (long)max);
      }

      public static short Clamp(bool abs, short value, short min, short max)
      {
         return (short)TypeParser.Clamp(
            abs, (long)value, (long)min, (long)max);
      }

      public static int Clamp(int value, int max)
      {
         return (int)TypeParser.Clamp(false, (long)value, (long)0, (long)max);
      }

      public static int Clamp(int value, int min, int max)
      {
         return (int)TypeParser.Clamp(
            false, (long)value, (long)min, (long)max);
      }

      public static int Clamp(bool abs, int value, int min, int max)
      {
         return (int)TypeParser.Clamp(abs, (long)value, (long)min, (long)max);
      }

      public static long Clamp(long value, long max)
      {
         return TypeParser.Clamp(false, value, (long)0, max);
      }

      public static long Clamp(long value, long min, long max)
      {
         return TypeParser.Clamp(false, value, min, max);
      }

      public static long Clamp(bool abs, long value, long min, long max)
      {
         if (abs) { min = Math.Abs(min); max = Math.Abs(max); }
         if (max == min) return min;
         if (max <  min) (max, min) = (min, max);
         return value < min ? min : value > max ? max : value;
      }

      public static float Clamp(float value, float max)
      {
         return TypeParser.Clamp(false, value, (float)0, max);
      }

      public static float Clamp(float value, float min, float max)
      {
         return TypeParser.Clamp(false, value, min, max);
      }

      public static float Clamp(
         bool   abs,
         float value,
         float min,
         float max)
      {
         if (float.IsNaN(value) || float.IsNaN(min) || float.IsNaN(max)) {
            return value;
         }
         if (abs) { min = Math.Abs(min); max = Math.Abs(max); }
         if (max == min) return min;
         if (max <  min) (max, min) = (min, max);
         return value < min ? min : value > max ? max : value;
      }

      public static double Clamp(double value, double max)
      {
         return TypeParser.Clamp(false, value, (double)0, max);
      }

      public static double Clamp(double value, double min, double max)
      {
         return TypeParser.Clamp(false, value, min, max);
      }

      public static double Clamp(
         bool   abs,
         double value,
         double min,
         double max)
      {
         if (double.IsNaN(value) || double.IsNaN(min) || double.IsNaN(max)) {
            return value;
         }
         if (abs) { min = Math.Abs(min); max = Math.Abs(max); }
         if (max == min) return min;
         if (max <  min) (max, min) = (min, max);
         return value < min ? min : value > max ? max : value;
      }

      public static decimal Clamp(decimal value, decimal max)
      {
         return TypeParser.Clamp(false, value, 0, max);
      }

      public static decimal Clamp(decimal value, decimal min, decimal max)
      {
         return TypeParser.Clamp(false, value, min, max);
      }

      public static decimal Clamp(
         bool    abs,
         decimal value,
         decimal min,
         decimal max)
      {
         if (abs) { min = Math.Abs(min); max = Math.Abs(max); }
         if (max == min) return min;
         if (max <  min) (max, min) = (min, max);
         return value < min ? min : value > max ? max : value;
      }

      public static ushort Clamp(ushort value, ushort max)
      {
         return (ushort)TypeParser.Clamp(
            (ulong)value, ulong.MinValue, (ulong)max);
      }

      public static ushort Clamp(ushort value, ushort min, ushort max)
      {
         return(ushort)TypeParser.Clamp((ulong)value, (ulong)min, (ulong)max);
      }

      public static uint Clamp(uint value, uint max)
      {
         return (uint)TypeParser.Clamp(
            (ulong)value, ulong.MinValue, (ulong)max);
      }

      public static uint Clamp(uint value, uint min, uint max)
      {
         return (uint)TypeParser.Clamp((ulong)value, (ulong)min, (ulong)max);
      }

      public static ulong Clamp(ulong value, ulong max)
      {
         return TypeParser.Clamp(value, ulong.MinValue, max);
      }

      public static ulong Clamp(ulong value, ulong min, ulong max)
      {
         if (max == min) return min;
         if (max <  min) (max, min) = (min, max);
         return value < min ? min : value > max ? max : value;
      }

      public static DateTime Clamp(DateTime value, DateTime max)
      {
         DateTime result = TypeParser.Clamp(value, DateTime.MinValue, max);
         return result;
      }

      public static DateTime Clamp(DateTime value, DateTime min, DateTime max)
      {
         long ticks = TypeParser.Clamp(value.Ticks, min.Ticks, max.Ticks);
         DateTime result = new (ticks);
         return result;
      }

      // TODO Guid Clamp
      public static Guid Clamp(bool asString, Guid value, Guid min, Guid max)
      {
         int result = (int)TypeParser.CompareGuids(asString, min, max);
         return result != 0 ? result > 0 ? max : min : value;
      }
      // TODO SID Clamp
      // TODO Uri Clamp
      // TODO UUID Clamp
      // TODO Enum Clamp
      // TODO IntPtr Clamp
      // TODO UIntPtr Clamp
      // DONE BigInteger Clamp
      // TODO Complex Clamp
      // TODO Tuple Clamp
      // TODO ValueTuple Clamp

      public static BigInteger Clamp(BigInteger value, BigInteger max)
      {
         return TypeParser.Clamp(false, value, BigInteger.Zero, max);
      }

      public static BigInteger Clamp(
         BigInteger value,
         BigInteger min,
         BigInteger max)
      {
         return TypeParser.Clamp(false, value, min, max);
      }

      public static BigInteger Clamp(
         bool       abs,
         BigInteger value,
         BigInteger min,
         BigInteger max)
      {
         if (abs) { min = BigInteger.Abs(min); max = BigInteger.Abs(max); }
         if (max == min) return min;
         if (max <  min) (max, min) = (min, max);
         return value < min ? min : value > max ? max : value;
      }

      public static Complex Clamp(Complex value, Complex max)
      {
         return TypeParser.Clamp(false, value, Complex.Zero, max);
      }

      public static Complex Clamp(Complex value, Complex min, Complex max)
      {
         return TypeParser.Clamp(false, value, min, max);
      }

      public static Complex Clamp(
         bool    abs,
         Complex value,
         Complex min,
         Complex max)
      {
         return new Complex(
            TypeParser.Clamp(abs, value.Real, min.Real, max.Real),
            TypeParser.Clamp(abs, value.Imaginary, min.Imaginary,
               max.Imaginary));
      }

      #endregion public static Clamp methods
   }
}
