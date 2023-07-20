// .·....1....·....2....·....3....·....4....·....5....·....6....·....7....·....8
// RuntimeProps.cs
// Copyright © William Edward Wesse
//
#nullable enable
using System;
using System.Collections;
using System.Globalization;
using System.Resources;
using System.Runtime.InteropServices;
using System.Timers;

namespace TypeHelp
{
    /// <summary>
    /// The r t prop.
    /// </summary>
    public enum RTProp
    {
        Is32Bit        = 0,
        Is32BitApp     = 1,
        Is32BitOS      = 2,
        Is32On64BitOS  = 3,
        Is64Bit        = 4,
        Is64BitApp     = 5,
        Is64BitOS      = 6,
        LongPointers   = 7,
        PreferInt32    = 8,
        Strict         = 9,
        //
        Nullity        = 0,
        NullName       = 1,
        SizeOfChar     = 2,
        SizeOfComplex  = 3,
        SizeOfDateTime = 4,
        SizeOfGuid     = 5,
        SizeOfPointer  = 6,
        SizeOfTimeSpan = 7,
        SizeOfVoid     = 8,
        TickCount      = 9,
    }

    public interface IRuntimeFeatures
    {
        public bool Is32Bit { get; }
        public bool Is32BitApp { get; }
        public bool Is32BitSys { get; }
        public bool Is32On64 { get; }
        public bool Is64Bit { get; }
        public bool Is64BitApp { get; }
        public bool Is64BitSys { get; }
        public bool LongPointers { get; }
        public bool PreferInt32 { get; }
        public bool Strict { get; }
    }

    public interface IRuntimeProperties
    {
        public Nullity Nullity { get; }
        public string NullName { get; }
        public int SizeOfChar { get; }
        public int SizeOfComplex { get; }
        public int SizeOfDateTime { get; }
        public int SizeOfGuid { get; }
        public int SizeOfPointer { get; }
        public int SizeOfTimeSpan { get; }
        public int SizeOfVoid { get; }
        public long TickCount { get; set; }
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct RTFeatures : IRuntimeFeatures
    {
        public const bool DEFAULT_PREFER_INT32 = false;
        public BitArray Items { get { return items; } }
        public bool Is32Bit { get { return items[0]; } }
        public bool Is32BitApp { get { return items[1]; } }
        public bool Is32BitSys { get { return items[2]; } }
        public bool Is32On64 { get { return items[3]; } }
        public bool Is64Bit { get { return items[4]; } }
        public bool Is64BitApp { get { return items[5]; } }
        public bool Is64BitSys { get { return items[6]; } }
        public bool LongPointers { get { return items[7]; } }
        public bool PreferInt32 { get { return items[8]; } }
        public bool Strict { get { return items[9]; } }

        public bool? Item(RTProp prop)
        {
            bool? value = null;
            if (Item(prop, out bool result)) {
                value = result;
            }
            return value;
        }

        public bool Item(RTProp prop, out bool value)
        {
            bool result = Enum.IsDefined(typeof(RTProp), prop);
            value = result
               ? items[(int)prop]
               : result;
            return result;
        }

        private bool Item(int index)
        {
            return (bool)items[index];
        }

        public RTFeatures(bool strict, bool? preferInt32)
        {
            bool prefInt32 = null != preferInt32 && (bool)preferInt32;
            bool is64BitApp = Environment.Is64BitProcess;
            bool is64BitSys = Environment.Is64BitOperatingSystem;

            items = new BitArray(new bool[] {
            !(is64BitApp || is64BitSys),  // Is32Bit
            !is64BitApp,                  // Is32BitApp
            !is64BitSys,                  // Is32BitSys
            is64BitSys && !is64BitApp,    // Is32On64
            is64BitApp && is64BitSys,     // Is64Bit
            is64BitApp,                   // Is64BitApp
            is64BitSys,                   // Is64BitSys
            is64BitApp,                   // LongPointers
            prefInt32,                    // PreferInt32
            strict,
         });
        }
        private readonly BitArray items;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct RTProps : IRuntimeProperties
    {
        public object[] Items { get { return items; } }
        public Nullity Nullity { get { return (Nullity)items[0]; } }
        public string NullName { get { return (string)items[1]; } }
        public int SizeOfChar { get { return (int)items[2]; } }
        public int SizeOfComplex { get { return (int)items[3]; } }
        public int SizeOfDateTime { get { return (int)items[4]; } }
        public int SizeOfGuid { get { return (int)items[5]; } }
        public int SizeOfPointer { get { return (int)items[6]; } }
        public int SizeOfTimeSpan { get { return (int)items[7]; } }
        public int SizeOfVoid { get { return (int)items[8]; } }
        public long TickCount {
            get { return (int)items[9]; }
            /*********************/
            set { items[9] = value; }
        }

        public RTProps(bool? zeroTickCount)
        {
            items = new object[] {
            Nullity.Value,                    // Nullity
            "null",
            sizeof(char),                     // SizeOfChar
            sizeof(double) * 2,               // SizeOfComplex
            sizeof(long),                     // SizeOfDateTime
            16,                               // SizeOfGuid
            Marshal.SizeOf(typeof(UIntPtr)),  // SizeOfPointer
            sizeof(long),                     // SizeOfTimeSpan
            0,                                // void
            ((null != zeroTickCount) && (bool)zeroTickCount)
               ? Environment.TickCount        // Initialization time
               : 0
         };
        }

        public int? Item(RTProp prop)
        {
            int? value = null;
            if (prop > RTProp.NullName && Item(prop, out int result)) {
                value = result;
            }
            return value;
        }

        public object? Item(RTProp prop, bool intValuesOnly)
        {
            bool result = IsDefined(prop);
            object value = result && (!intValuesOnly || prop > RTProp.NullName)
               ? (int)items[(int)prop]
               : 0;
            return value;
        }

        public bool Item(RTProp prop, out int value)
        {
            bool result = IsDefined(prop) && prop > RTProp.NullName;
            value = result
               ? (int)items[(int)prop]
               : 0;
            return result;
        }

        private static bool IsDefined(int rtProp)
        {
            return Enum.IsDefined(typeof(RTProp), rtProp);
        }

        private static bool IsDefined(RTProp prop)
        {
            return Enum.IsDefined(typeof(RTProp), prop);
        }

        private int Item(int index)
        {
            if (index < (int)RTProp.NullName || index >= items.Length) {
                return 0;
            }
            return (int)items[index];
        }

        private readonly object[] items;
    }

    [StructLayout(LayoutKind.Sequential)]
    public readonly struct RuntimeProps : IRuntimeFeatures, IRuntimeProperties
    {
        public RuntimeProps(bool? preferInt32)
        {
            rtps = new RTProps();
            rtfs = new RTFeatures(false, preferInt32);
            rtci = CultureInfo.InvariantCulture;
            rtrm = new ResourceManager(
               typeof(string).Name,
               typeof(RuntimeProps).Assembly);
        }

        public RuntimeProps(bool strict, bool? preferInt32)
        {
            rtps = new RTProps();
            rtfs = new RTFeatures(strict, preferInt32);
            rtci = CultureInfo.InvariantCulture;
            rtrm = new ResourceManager(
               typeof(string).Name,
               typeof(RuntimeProps).Assembly);
        }

        public CultureInfo Culture { get { return rtci; } }
        public RTFeatures Features { get { return rtfs; } }
        public RTProps Properties { get { return rtps; } }

        public static readonly string[] Formats = new string[] {
         "{0}", "{0}",
         "{0}{1}", "{0}{1}{2}", "{0}{1}{2}{3}",
         "{0}{1}{2}{3}{4}", "{0}{1}{2}{3}{4}{5}"
      };

        public static readonly string[] FormatSPs = new string[] {
         "{0}", "{0}",
         "{0} {1}", "{0} {1} {2}", "{0} {1} {2} {3}",
         "{0} {1} {2} {3} {4}", "{0} {1} {2} {3} {4} {5}"
      };

        public bool Is32Bit { get { return (bool)rtfs.Is32Bit; } }
        public bool Is32BitApp { get { return (bool)rtfs.Is32BitApp; } }
        public bool Is32BitSys { get { return (bool)rtfs.Is32BitSys; } }
        public bool Is32On64 { get { return (bool)rtfs.Is32On64; } }
        public bool Is64Bit { get { return (bool)rtfs.Is64Bit; } }
        public bool Is64BitApp { get { return (bool)rtfs.Is64BitApp; } }
        public bool Is64BitSys { get { return (bool)rtfs.Is64BitSys; } }
        public bool LongPointers { get { return (bool)rtfs.LongPointers; } }
        public bool PreferInt32 { get { return (bool)rtfs.PreferInt32; } }
        public bool Strict { get { return (bool)rtfs.Strict; } }
        public Nullity Nullity { get { return (Nullity)rtps.Nullity; } }
        public string NullName { get { return (string)rtps.NullName; } }
        public int SizeOfChar { get { return (int)rtps.SizeOfChar; } }
        public int SizeOfComplex { get { return (int)rtps.SizeOfComplex; } }
        public int SizeOfDateTime { get { return (int)rtps.SizeOfDateTime; } }
        public int SizeOfGuid { get { return (int)rtps.SizeOfGuid; } }
        public int SizeOfPointer { get { return (int)rtps.SizeOfPointer; } }
        public int SizeOfTimeSpan { get { return (int)rtps.SizeOfTimeSpan; } }
        public int SizeOfVoid { get { return (int)rtps.SizeOfVoid; } }
        public long TickCount {
            get { return (int)rtps.TickCount; }
            /************************/
            set { rtps.TickCount = value; }
        }

        public string ResourceString(string name)
        {
            string? result;
            try {
                result = rtrm.GetString(name, this.Culture);
                if (null == result) {
                    return string.Empty;
                }
                int i = result.IndexOf("{0");
                if (i >= 0 && result.IndexOf("}") >= i) {
                    int j = result.IndexOf("}");
                    result = string.Format(Formats[3],
                       result[..i],
                       "<missing>",
                       result[(j + 1)..]);
                }
            }
            catch { result = name; }
            return result;
        }

        public string ResourceString(string name, string argument)
        {
            string? result;
            try {
                result = rtrm.GetString(name, this.Culture);
                if (null == result) {
                    return string.Empty;
                }
                if (result.Contains("{0")) {
                    result = string.Format(result, argument);
                } else {
                    result = string.Format(FormatSPs[2], result, argument);
                }
            }
            catch {
                result = string.Format(FormatSPs[2], name, argument);
            }
            return result;
        }

        private static void on_timed_event(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("Elapsed event at {0}", e.SignalTime);
        }

        private readonly RTProps rtps;
        private readonly RTFeatures rtfs;
        private readonly CultureInfo rtci;
        private readonly ResourceManager rtrm;
    }
}
