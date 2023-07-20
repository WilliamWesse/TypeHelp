// Copyright (c) William Edward Wesse. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.
// This is a redaction of ManagedReference.extension.js
// Portions Copyright (c) Microsoft. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.

const Fetcher = require('./PropertyFetcher.js').Fetcher;
/**
 * This method will be called at the start of exports.transform in ManagedReference.html.primary.js
 */
export function preTransform(model) {
    if (model.type.toLowerCase() === 'enum') {
        model = applyTransform(model);
    }
    return model;
}

/**
 * This method will be called at the end of exports.transform in ManagedReference.html.primary.js
 */
export function postTransform (model) {
    return model;
}

function applyTransform(model) {
    if (model.type.toLowerCase() !== 'enum') {
        return model;
    }
    let count = 0;
    try {
        const sep = ''.padEnd(80, '#');
        console.warn(sep);
        const opts = Fetcher.getOptions();
        console.table(opts);
        opts.sorts.forEach(item => {
            console.table(item);
        });
        console.log(sep);
        const info = Fetcher.getPropertyInfo(model);
        console.table(info);
        model.children.forEach(function (field) {
            if (is_valid(field) && is_valid(field.syntax)) {
                count++;
            }
        });
        console.log(sep);
        describe(model, pluralize(count, ' element', ''));
        console.warn(sep);
        count = enumerationTransform(model);
    } catch (e) {
        console.error(e);
        throw e;
    }
    return model;
}

function describe(item, label) {

    // Overwrite Files | docfx
    // https://dotnet.github.io/docfx/tutorial/intro_overwrite_files.html#data-model-inside-docfx

    let text = ('# Item:     ' + label + ': ');
    text = text.padEnd(10, ' ');
    console.warn(text + get_valid(item.type, '?') +
        ' ' + get_valid(item.uid, '?') + ' (uid)');
    console.warn('# Type:     ' + item);
    if (is_valid(item.attributes)) {
        let attribs = '';
        item.attributes.forEach(function (attrib) {
            attribs += ' ' + attrib.type;
        });
        console.warn('# Attribs: ' + attribs);
    }
}

function enumerationTransform(enumeration) {
    var count = 0;

    if (!is_valid(enumeration.children)) {
        console.warn(enumeration.uid + ' has no children.');
        return count;
    }
    try {
        let width = 0;

        enumeration.children.forEach(function (field) {
            if (is_valid(field) && is_valid(field.syntax)) {
                count++;
                if (width < field.name.length) {
                    width = field.name.length;
                }
            }
        });
        width++;
        const fmtHex = hasFlagsAttribute(enumeration);
        const length = hexDisplaySize(enumeration);
        const spacer = fmtHex || enumeration.verbose ? '\xa0' : ' ';

        enumeration.children.forEach(function (field) {
            //console.table(field);
            if (is_valid(field) && is_valid(field.syntax)) {
                //const fieldName = field.name.toString().padEnd(width, spacer[0]);
                describe(field, field.syntax);
                //field.syntax.content[0].value = fieldName + '=' + spacer +
                //    formatValue(enumeration, integerValue(field), fmtHex, length);
            }
        });
    } catch (e) {
        console.warn(e);
    }
    return count;
}

// function fieldCount(enumeration) {
//    var count = 0;
//    try {
//      enumeration.children.forEach(function (field) {
//         if (field.type.toLowerCase() === 'field') {
//            count++;
//         }
//      });
//    } catch (e) {
//      errorValue = e;
//    }
//    return count;
// }

function formatValue(enumeration, value, fmtHex, length) {
    var result = value.toString(16).toUpperCase();

    if (result.length < length) {
        result = '0'.repeat(length - result.length) + result;
    }
    result = '0x' + result;
    if (fmtHex) {
        if (enumeration.verbose) {
            result += '\xa0' + surround(value.toString(), '(');
        }
    } else {
        result = enumeration.verbose
            ? value.toString() + '\xa0' + surround(result, '(')
            : value.toString();
    }
    return result;
}

function get_valid(tag, defaultText) {
    return is_valid(tag)
        ? tag
        : is_valid(defaultText) ? defaultText : '?';
}

function hasAttribute(item, attributeName) {
    var result = false;
    if (!is_valid(item.attributes) || item.attributes.length === 0) {
        return result;
    }
    try {
        let attrib = attributeName.toLowerCase();
        const tail = 'attribute';
        if (!attrib.endsWith(tail)) {
            attrib += tail;
        }
        item.attributes.forEach(function (attribute) {
            let type = attribute.type.toLowerCase();
            if (!result && (type === attrib || type.startsWith(attrib))) {
                result = true;
            }
        });
    } catch (e) {
        result = false;
        console.warn(e);
    }
    return result;
}

function hasFlagsAttribute(enumeration) {
    var result = false;
    try {
        result = enumeration.syntax.content.indexOf('[Flags]') >= 0;
        if (!result) {
            result = hasAttribute(enumeration, 'System.FlagsAttribute');
        }
    } catch (e) {
        console.warn(e);
    }
    return result;
}

function hexDisplaySize(enumeration) {
    let maximum = 0;
    try {
        enumeration.children.forEach(function (field) {
            if (field.type.toLowerCase() === 'field') {
                let value = Math.abs(integerValue(field));
                if (maximum < value) {
                    maximum = value;
                }
            }
        });
    } catch (e) {
        console.warn(e);
    }
    var nibbles = 0;    // the nibble (4-bit chunk) count
    while (maximum !== 0) {
        maximum >>>= 4; // unsigned shift
        nibbles++;
    }
    if (nibbles === 0) {
        nibbles = 2;    // Never zero
    } else if ((nibbles & 1) !== 0) {
        nibbles++;      // Always an even number
    }
    return nibbles;
}

function integerValue(field) {
    if (null === field.syntax || typeof field.syntax === 'undefined') {
        console.warn(integerValue.name + ': empty integer token');
    }
    const scont = field.syntax.content.toString();
    const index = scont.indexOf('=') + 1;
    const empty = '~';
    if (index <= 0) {
        console.warn(integerValue.name + ': ' +
            pluralize(scont.length, ' byte', ' = ') +
            surround(scont, empty));
    }
    let token = scont.substring(index).trim();
    var value = Number.parseInt(token);
    if (Number.isNaN(value) || !Number.isSafeInteger(value)) {
        console.warn(integerValue.name + ': is ' +
            Number.isNaN(value) ? 'NaN.' : 'an unsafe integer.');
    }
    // field.syntax.content = field.syntax.content.replace(' ', '\xa0');
    return value;
}

function is_string(candidate, requireContent) {
    return typeof candidate === 'string' &&
        requireContent ? candidate.length > 0 : true;
}

function is_valid(item) {
    return typeof item !== 'undefined' && null !== item;
}

const _es_tails = ['ch', 'o', 'ss', 'sh', 's', 'x', 'z'];

function pluralize(numeric, label, tail) {
    if (numeric !== 1) {
        if (label.endsWith('y')) {
            label = label.substring(0, label.length - 1) + 'ies';
        } else {
            let plural = 's';
            _es_tails.forEach(function (suffix) {
                if (label.endsWith(suffix)) {
                    plural = 'es';
                }
            });
            label += plural;
        }
    }
    return numeric.toString() + ' ' + label + tail;
}

function surround(target, mark) {
    if (!is_string(mark)) {
        mark = "'";
    }
    let head = is_string(mark, true) ? mark.trim() : "'";
    if (head.length === 0) {
        head = ' ';
    }
    let tail = head;
    switch (head) {
        case '(': tail = ')'; break;
        case '<': tail = '>'; break;
        case '[': tail = ']'; break;
        case '{': tail = '}'; break;
    }
    let body;
    try {
        switch (typeof target) {
            case 'string': body = target; break;
            case 'object':
                body = null !== target ? target.toString() : 'null'; break;
            case 'function':
                body = Object.prototype.toString.call(target); break;
            case 'undefined': body = typeof target; break;
            default: body = target.toString(); break;
        }
    } catch (e) {
        console.warn(e);
        body = 'undefined';
    }
    const result = mark + body + tail;
    return result;
}
