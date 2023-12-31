﻿# Introduction to TypeHelp

TypeHelp is a .NET 7.0 Framework DLL with XML documented classes, structs and enums for type parsing and analysis, and incorporates *DocFx* configuration, UI decoration extensions, and test facilities. It is currently available under the MIT License.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
- See the [TypeHelp](http://localhost:5000/api/TypeHelp.html)&#xA0;Namespace documentation extracted from the project source comments.

## General Features

### Unicode character enumerations

* `Cmp`  Comparison [`LSS <`, `LEQ ≤`,..., `GEQ ≥`]
* `UCat`  [`Lu`, `Ll`,...] as a short label alias for `UnicodeCategory`
* `UCGeneralUse`  [`BrokenBar ¦`, `Copyright ©`,..., `BOM_LE 0xFFFE`]
* `UCGreek`  [Alpha, Beta,..., omega]
* `UCMath`  [Circled, Comparison, Group, Logical, Op, Set, Square,...]
* `ZeroWidthChars`  [Invisible, Join, Mark, Shape, Space, Swap,...]
* `ZeroWidthSpaces`  [Space, Non-Joiner, Joiner]

### An Extended TypeCode enumeration
*`TypeCoex`* - defines the following types:

#### Well-known String types
* `Base64`
* `UUID`
* `Guid`
* `SID`
* `Uri`
* `BinHex`
* `Uuencode`

#### Additional data types
* `Enum`
* `TimeSpan`
* `IntPtr`
* `UIntPtr`
* `Complex`
* `BigInteger`
* `BitArray`
* `Tuple`
* `ValueTuple`
