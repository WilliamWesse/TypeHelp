// Copyright (c) William Edward Wesse. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.

const pf = require('../docs/templates/enumerations/Property_Fetcher.js');

const sep = ''.padEnd(80, '#');
console.log(sep);
console.log(Object.getPrototypeOf(pf.Fetcher).constructor.name);
console.log(sep);

const opts = pf.Fetcher.getOptions();
const info = pf.Fetcher.getPropertyInfo({
    name:   "name",
    value:  0,
    values: [0, 1]
});

console.table(opts);
opts.sorts.forEach(item => {
    console.table(item);
});
console.log(sep);
console.table(info);
console.log(sep);

// TypeScript: Handbook - Enums
// https://www.typescriptlang.org/docs/handbook/enums.html
//
// Reverse mappings
// https://www.typescriptlang.org/docs/handbook/enums.html#reverse-mappings

function MakeEnum(elementNames) {
    const enumeration = {};
    for (let i = 0; i < elementNames.length; i++) {
        const typeName = elementNames[i];
        enumeration[enumeration[typeName] = i] = typeName;
    }
    return enumeration;
}
const typeElementNames = [
    'boolean', 'bigint', 'function', 'number',
    'object', 'string', 'symbol', 'undefined' // 'null', 'class'
];
const Types = MakeEnum(typeElementNames);
const element = Types.object;
const elementName = Types[element];
const typesExample = {
    name: elementName,
    value: element,
    reverse: Types[elementName],
    mapping: Types[elementName] === element
};

console.log('Types \'enum\' with example of reverse mapping');
console.log(sep);
console.table(typesExample);
console.log(sep);
console.table(Types);
console.log(sep);
console.log('');

// Object.getPrototypeOf(obj).constructor.name
// Fundamental objects
//   Object
//   Function
//   Boolean
//   Symbol
// Error Objects
//   Error
//   AggregateError
//   EvalError
//   RangeError
//   ReferenceError
//   SyntaxError
//   TypeError
//   URIError
// Numbers and dates
//   Number
//   BigInt
//   Math
//   Date
// Text processing
//   String
//   RegExp
// Indexed collections
//   Array
//   Int8Array
//   Uint8Array
//   Uint8ClampedArray
//   Int16Array
//   Uint16Array
//   Int32Array
//   Uint32Array
//   BigInt64Array
//   BigUint64Array
//   Float32Array
//   Float64Array
// Keyed collections
//   Map
//   Set
//   WeakMap
//   WeakSet
// Structured data
//   ArrayBuffer
//   SharedArrayBuffer
//   DataView
//   Atomics
//   JSON
// Memory Management
//   WeakRef
//   FinalizationRegistry
// Control abstraction objects
//   Iterator
//   AsyncIterator
//   Promise
//   GeneratorFunction
//   AsyncGeneratorFunction
//   Generator
//   AsyncGenerator
//   AsyncFunction
//   Reflection
//   Reflect
//   Proxy
// Internationalization
//   Intl
//     Collator
//     DateTimeFormat
//     DisplayNames
//     DurationFormat
//     ListFormat
//     Locale
//     NumberFormat
//     PluralRules
//     RelativeTimeFormat
//     Segmenter
