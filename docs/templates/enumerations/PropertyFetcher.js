// Copyright (c) William Edward Wesse. All rights reserved. Licensed under the MIT license. See LICENSE file in the project root for full license information.
// PropertyFetcher.js
// This is a redaction of the SimplePropertyRetriever code at:
// Enumerability and ownership of properties - JavaScript | MDN
// https://developer.mozilla.org/en-US/docs/Web/JavaScript/Enumerability_and_ownership_of_properties#obtaining_properties_by_enumerabilityownership

class PropertyFetcher {

    constructor(
        /** @type {object?}  */ options,
        /** @type {boolean?} */ useDefaultOnError) {

        this.#_options.ranks.length = 0;
        if (null !== options && options instanceof Object) {
            this.setOptions(options, !!useDefaultOnError);
        } else {
            this.#_set_check_include_fn();
        }
    }

    getOptions(/** @type {boolean} */ defaultOrCurrent) {

        const options = !!defaultOrCurrent
            ? {
                defaultOnError: true,
                enumerables:    true,
                includeSymbols: true,
                inherited:      true,
                nonEnumerables: true,
                outputRootObj:  true,
                ownProperties:  true,
                ranks:          [],
                sortBy:         ['rank', 'name'],
                sorts:          [...this.#_get_sorts(this.#_default_sorts)]
            }
            : {
                defaultOnError: this.#_options.defaultOnError,
                enumerables:    this.#_options.enumerables,
                includeSymbols: this.#_options.includeSymbols,
                inherited:      this.#_options.inherited,
                nonEnumerables: this.#_options.nonEnumerables,
                outputRootObj:  this.#_options.outputRootObj,
                ownProperties:  this.#_options.ownProperties,
                ranks:          [...this.#_options.ranks],
                sortBy:         [...this.#_options.sortBy],
                sorts:          [...this.#_get_sorts(this.#_options.sorts)],
            };
        return options;
    }

    getPropertyInfo(/** @type {any} */ obj) {

        let info = [];
        try {
            info = this.#_get_property_info_set(
                obj,
                this.#_options.ownProperties,
                this.#_options.inherited);
            const featureSorts = [];
            for (let i = 0; i < this.#_options.sortBy.length; i++) {
                const feature = this.#_features.indexOf(this.#_options.sortBy[i]);
                if (feature >= 0 && feature <= 2) {
                    featureSorts.push(feature);
                }
            }
            if (featureSorts.length > 0) {
                const compare_callback_fn = (
                    /** @type {{ rank: number; name: string; type: string; }} */ a,
                    /** @type {{ rank: number; name: string; type: string; }} */ b) => {
                    let comp = 0;
                    for (let i = 0; i < featureSorts.length; i++) {
                        switch (featureSorts[i]) {
                            case 0: comp = this.#_compare_by_name_callback(a, b); break;
                            case 1: comp = this.#_compare_by_rank_callback(a, b); break;
                            case 2: comp = this.#_compare_by_type_callback(a, b); break;
                            default: continue;
                        }
                        if (comp === 0 && i < featureSorts.length - 1) {
                            continue;
                        }
                        break;
                    }
                    return comp;
                };
                info.sort(compare_callback_fn);
            }
            return info;
        } catch (e) {
            // info = [];
            console.error('getPropertyInfo');
            throw e;
        }
    }

    getPropertyNames(/** @type {any} */ obj) {

        let names = [];
        try {
            names = this.#_get_property_names(
                obj,
                this.#_options.ownProperties,
                this.#_options.inherited);
            const sort = this.#_features.indexOf(this.#_options.sortBy[0]) >= 0;
            const down = this.#_options.sorts[0].descend;
            if (sort || down) {
                names.sort();
                if (down) {
                    names.reverse();
                }
            }
        } catch (e) {
            // names = [];
            console.error('getPropertyNames');
            throw e;
        }
        return names;
    }

    includeCheckCallback(
        /** @type {any}    */ obj,
        /** @type {string} */ propertyName) {

        return this.#_is_valid(obj) && this.#_is_valid_token(propertyName);
    }

    setOptions(
        /** @type {any}     */ options,
        /** @type {boolean} */ useDefaultOnError) {

        let valid = this.#_is_valid(options);
        if (!valid) {
            if (!useDefaultOnError) {
                return false;
            }
            valid = true;
            options = this.getOptions(false);
        }
        useDefaultOnError = this.#_is_valid(useDefaultOnError)
            ? !!useDefaultOnError
            : this.#_options.defaultOnError;
        valid = valid || this.#_is_valid(useDefaultOnError);
        this.#_options.defaultOnError = useDefaultOnError;
        this.#_options.enumerables    = this.#_value_of(options, 'enumerables',    valid);
        this.#_options.inherited      = this.#_value_of(options, 'inherited',      valid);
        this.#_options.nonEnumerables = this.#_value_of(options, 'nonEnumerables', valid);
        this.#_options.outputRootObj  = this.#_value_of(options, 'outputRootObj',  valid);
        this.#_options.ownProperties  = this.#_value_of(options, 'ownProperties',  valid);
        this.#_options.ranks          = this.#_value_of(options, 'ranks',          []);
        try {
            if (this.#_has_property_of_type(options, 'sorts', 'object')) {
                for (const sort of options.sorts) {
                    if (this.#_is_valid_sort(sort)) {
                        const entry = Object.assign({}, sort);
                        let index = this.#_features.indexOf(entry.feature);
                        if (index >= 0) {
                            this.#_options.sorts[index] = entry;
                        }
                    }
                }
            }
        } catch (e) {
            console.error('setOptions');
            throw e;
        }
        this.#_set_check_include_fn();
        return true;
    }

    setPropertyNames(
        /** @type {boolean}  */ exclude,
        /** @type {string[]} */ propertyNames) {

        try {
            this.#_options.sorts[0].exclude = !!exclude;
            this.#_options.sorts[0].lookups.length = 0;
            const names = this.#_sanitize_strings(propertyNames);
            this.#_options.sorts[0].lookups.push(...names);
            return this.#_options.sorts[0].lookups.length;
        } catch (e) {
            console.error('setPropertyNames');
            throw e;
        }
    }

    setPropertyTypes(
        /** @type {boolean}  */ exclude,
        /** @type {string[]} */ typeNames) {

        try {
            this.#_options.sorts[2].exclude = !!exclude;
            this.#_options.sorts[2].lookups.length = 0;
            const names = this.#_sanitize_strings(typeNames);

            for (let i = 0; i < names.length; i++) {
                const typeName = names[i];
                if (this.#_default_sorts[2].lookups.indexOf(typeName) >= 0) {
                    this.#_options.sorts[2].lookups.push(typeName);
                }
            }
            return this.#_options.sorts[2].lookups.length;
        } catch (e) {
            console.error('setPropertyTypes');
            throw e;
        }
    }

    #_compare_by_name_callback(
        /** @type {{ rank: number; name: string; type: string; }} */ a,
        /** @type {{ rank: number; name: string; type: string; }} */ b) {

        const comp = a.name.localeCompare(b.name);
        if (comp === 0) {
            return 0;
        }
        if (this.#_options.sorts[0].descend) {
            return comp < 0 ? 1 : -1;
        }
        return comp > 0 ? 1 : -1;
    }

    #_compare_by_rank_callback(
        /** @type {{ rank: number; name: string; type: string; }} */ a,
        /** @type {{ rank: number; name: string; type: string; }} */ b) {

        if (a.rank === b.rank) {
            return 0;
        }
        if (this.#_options.sorts[1].descend) {
            return a.rank < b.rank ? 1 : -1;
        }
        return a.rank > b.rank ? 1 : -1;
    }

    #_compare_by_type_callback(
        /** @type {{ rank: number; name: string; type: string; }} */ a,
        /** @type {{ rank: number; name: string; type: string; }} */ b) {

        const comp = a.type.localeCompare(b.type);
        if (comp === 0) {
            return 0;
        }
        if (this.#_options.sorts[2].descend) {
            return comp < 0 ? 1 : -1;
        }
        return comp > 0 ? 1 : -1;
    }

    #_get_property_info_set(
        /** @type {any}     */ obj,
        /** @type {boolean} */ owned,
        /** @type {boolean} */ inherited) {

        const infoSet = [];
        let target = obj;
        let ranked = 0;
        this.#_rank = 0;

        if (!!inherited) {
            do {
                target = Object.getPrototypeOf(target);
                ranked++;
            } while (target);
        }
        target = obj;
        do {
            if (!!owned) {
                Object.getOwnPropertyNames(target).forEach((/** @type {string} */ propertyName) => {
                    if (this.includeCheckCallback(target, propertyName)) {
                        const descriptor = Object.getOwnPropertyDescriptor(target, propertyName);
                        if (descriptor) {
                            const isArray = descriptor.value instanceof Array;
                            const length  = isArray ? descriptor.value.length : 1;
                            const element = isArray
                                ? length > 0 ? descriptor.value[0] : null
                                : descriptor.value;
                            infoSet.push({
                                name:         propertyName,
                                configurable: descriptor.configurable,
                                descriptor:   descriptor,
                                enumerable:   descriptor.enumerable,
                                hasGetter:    this.#_is_valid(descriptor.get),
                                hasSetter:    this.#_is_valid(descriptor.set),
                                hasValue:     this.#_is_valid(element),
                                instanceRank: this.#_rank,
                                isArray:      isArray,
                                isNull:       null === element,
                                isUndefined:  typeof element === 'undefined',
                                numElements:  length,
                                typeName:     typeof element,
                                writable:     descriptor.writable,
                                value:        descriptor.value
                            });
                        }
                    }
                });
            }
            ranked--;
            this.#_rank++;
            if (!inherited || (
                ranked < 1 && !this.#_options.outputRootObj)) {
                break;
            }
            owned = true;
            target = Object.getPrototypeOf(target);
        } while (target);
        this.#_rank = 0;
        return infoSet;
    }

    // Inspired by http://stackoverflow.com/a/8024294/271577
    #_get_property_names(
        /** @type {any}      */ obj,
        /** @type {boolean}  */ owned,
        /** @type {boolean}  */ inherited,
        /** @type {function} */ checkIncludeFn) {

        const list = [];
        const info = this.#_get_property_info_set(obj, owned, inherited);
        info.forEach((/** @type {any} */ entry) => {
            if (checkIncludeFn(obj, entry.name)) {
                list.push(entry.name);
            }
        });
        return list;
    }

    #_get_sorts(/** @type {any[]} */ sorts) {
        const results = [];
        for (const sort of sorts) {
            const entry = Object.assign({}, sort);
            results.push(entry);
        }
        return results;
    }

    #_has_property_of_type(
        /** @type {any}    */ obj,
        /** @type {string} */ propertyName,
        /** @type {string} */ typeName) {

        const lowName = typeName.toLowerCase();
        if (this.#_default_sorts[2].lookups.indexOf(lowName) >= 0 &&
            Object.keys(obj).indexOf(propertyName) >= 0) {
            return typeof obj[propertyName] === typeName;
        }
        return false;
    }

    #_include_property(/** @type {string} */ propertyName) {

        if (!this.#_include_rank()) {
            return false;
        }
        let listed = false;
        for (let i = 0; i < this.#_options.sorts[1].lookups.length; i++) {
            if (this.#_options.sorts[1].asRegex[i]) {
                listed = this.#_options.sorts[1].asRegex[i]
                    ? this.#_options.sorts[1].lookups[i].test(propertyName)
                    : this.#_options.sorts[1].lookups[i].indexOf(propertyName) >= 0;
                if (listed) {
                    break;
                }
            }
        }
        if (this.#_options.sorts[1].exclude) {
            listed = !listed;
        }
        return listed;
    }

    #_include_rank() {

        const index = this.#_options.ranks.indexOf(this.#_rank);
        switch (this.#_options.ranks.length) {
            case 0: return true;
            case 1: return index >= 0 || this.#_options.ranks[0] < 0;
            case 2:
                return this.#_options.ranks[0] > this.#_options.ranks[1]
                    ?   this.#_rank >= this.#_options.ranks[1] &&
                        this.#_rank <= this.#_options.ranks[0]
                    :   this.#_rank >= this.#_options.ranks[0] &&
                        this.#_rank <= this.#_options.ranks[1];
            default: return index >= 0;
        }
    }

    #_is_valid(/** @type {any} */ item) {

        return typeof item !== 'undefined' && null !== item;
    }

    #_is_valid_array(
        /** @type {any}     */ item,
        /** @type {string}  */ typeName,
        /** @type {boolean} */ requireAllValues) {

        let result = this.#_is_valid(item) &&
            item instanceof Array && item.length > 0;
        if (result) {
            const lowName = typeName.toLocaleLowerCase();
            let count = 0;
            item.forEach((/** @type {any} */ entry) => {
                const entryType = typeof entry;
                if (entryType.toLocaleLowerCase() === lowName) count++;
            });
            result = requireAllValues
                ? count === item.length
                : count > 0;
        }
        return result;
    }

    #_is_valid_regex(/** @type {any[]} */ obj) {

        let result = false;
        let testee = null;
        try {
            let flags = obj instanceof String
                ? 'g' : obj instanceof RegExp
                    ? obj.flags : ' ';
            if (flags !== ' ') {
                testee = new RegExp(obj.toString(), flags);
            }
        } catch {
            testee = null;
        }
        result = testee instanceof RegExp;
        return result;
    }

    #_is_valid_sort(/** @type {any} */ sort) {

        if (!this.#_is_valid(sort)) {
            return false;
        }
        try {
            if (this.#_has_property_of_type(sort, 'descend', 'boolean') &&
                this.#_has_property_of_type(sort, 'exclude', 'boolean') &&
                this.#_has_property_of_type(sort, 'feature', 'string') &&
                this.#_has_property_of_type(sort, 'lookups', 'object')) {

                const index = this.#_features.indexOf(sort.feature);
                let result = true;
                let i = 0;
                switch (index) {
                    case 0:
                        while (i < sort.lookups.length && result === true) {
                            const rank = sort.lookups[i];
                            if (typeof rank !== 'number') {
                                result = false;
                            } else i++;
                        }
                        break;
                    case 1:
                        while (i < sort.lookups.length && result === true) {
                            const propertyName = sort.lookups[i];
                            if (!this.#_is_valid_token(propertyName)) {
                                result = false;
                            } else i++;
                        }
                        break;
                    case 2:
                        while (i < sort.lookups.length && result === true) {
                            const typeName = sort.lookups[i];
                            if (this.#_default_sorts[2].lookups.indexOf(typeName) < 0) {
                                result = false;
                            } else i++;
                        }
                        break;
                }
                return i === sort.lookups.length;
            }
        } catch (e) {
            console.error('_is_valid_sort');
            throw e;
        }
    }

    #_is_valid_string(
        /** @type {any}     */ item,
        /** @type {boolean} */ trimItem,
        /** @type {boolean} */ requireContent,
        /** @type {boolean} */ requireAllNonWhite) {

        let result = item instanceof String;
        if (result) {
            const token = trimItem ? item.trim() : item;
            if (!!requireContent || !!trimItem) {
                result = !!requireContent ? token.length > 0 : true;
            }
            if (result && !!requireAllNonWhite) {
                const re = /\s/;
                result = !re.test(token);
            }
        }
        return result;
    }

    #_is_valid_token(/** @type {any} */ item) {

        let result = item instanceof String && item.length > 0;
        if (result) {
            result = !/\s/.test(item);
        }
        return result;
    }

    // property checker callbacks
    #_properties_enumerable_callback(
        /** @type {any}    */ obj,
        /** @type {string} */ propertyName) {

        return this.#_include_property(propertyName) &&
            Object.prototype.propertyIsEnumerable.call(obj, propertyName);
    }

    #_properties_enumerable_or_not_callback(
        /** @type {any}    */ obj,
        /** @type {string} */ propertyName) {

        return !this.#_include_property(propertyName) && (
            Object.keys(obj).indexOf(propertyName) >= 0 ||
            this.#_is_valid(Object.getOwnPropertyDescriptor(obj, propertyName)));
    }

    #_properties_not_enumerable_callback(
        /** @type {any}    */ obj,
        /** @type {string} */ propertyName) {

        return this.#_include_property(propertyName) &&
            !Object.prototype.propertyIsEnumerable.call(obj, propertyName);
    }

    #_sanitize_string(
        /** @type {string}  */ token,
        /** @type {boolean} */ trim,
        /** @type {boolean} */ requireAllNonWhite) {

        try {
            if (this.#_is_valid_string(token, trim, true, requireAllNonWhite)) {
                return token;
            }
        } catch (e) {
            console.error('_sanitize_string');
            throw e;
        }
        return '';
    }

    #_sanitize_strings(/** @type {string[]} */ tokens) {

        var sanitized = [];
        if (this.#_is_valid_array(tokens, 'string', true)) {
            try {
                tokens.forEach((/** @type {string} */ item) => {
                    const token = this.#_sanitize_string(item, true, true);
                    if (token.length > 0) {
                        sanitized.push(token);
                    }
                });
            } catch (e) {
                // sanitized = [];
                console.error('_sanitize_strings');
                throw e;
            }
        }
        return sanitized;
    }

    #_set_check_include_fn() {

        if (this.#_options.nonEnumerables) {
            this.includeCheckCallback = this.#_options.enumerables
                ? this.#_properties_enumerable_or_not_callback
                : this.#_properties_not_enumerable_callback;
            return;
        }
        this.includeCheckCallback = this.#_properties_enumerable_callback;
    }

    #_value_of(
        /** @type {any}    */ obj,
        /** @type {string} */ propertyName,
        /** @type {any}    */ defaultValue) {

        var result = defaultValue;
        if (this.#_is_valid(obj)) {
            try {
                const names = Object.getOwnPropertyNames(obj);
                if (names.indexOf(propertyName) >= 0) {
                    result = Object.getOwnPropertyDescriptor(obj, propertyName)?.value;
                    if (!this.#_is_valid(result)) {
                        result = defaultValue;
                    }
                }
            } catch (e) {
                // result = false;
                console.error('_value_of');
                throw e;
            }
        }
        if (defaultValue instanceof Boolean) {
            result = !!result;
        }
        return result;
    }

    #_create_sort(
        /** @type {boolean}   */ exclude,
        /** @type {number}    */ feature,
        /** @type {boolean[]} */ asRegex,
        /** @type {any[]}     */ lookups) {

        if (feature < 0) {
            feature = 0;
        } else if (feature >= this.#_features.length) {
            feature = this.#_features.length - 1;
        }
        var sort = {
            descend: false,
            exclude: exclude,
            feature: this.#_features[feature],
            asRegex: [false],
            lookups: ['']
        };
        for (let index = 0; index < lookups.length; index++) {
            var lookup = null;
            let aRegex = index < asRegex.length ? asRegex[index] : false;
            const fRegExp = lookups[index] instanceof RegExp;
            const fString = lookups[index] instanceof String;
            if (fRegExp || fString) {
                if (aRegex || fRegExp) {
                    aRegex = this.#_is_valid_regex(lookups[index]);
                    if (aRegex) {
                        lookup = lookups[index];
                    }
                }
                if (!aRegex && fString &&
                    this.#_is_valid_string(lookups[index], false, true, false)) {
                    lookup = lookups[index];
                }
            }
            if (null !== lookup) {
                sort.lookups.push(lookup);
                sort.asRegex.push(aRegex);
            }
        }
        return sort;

    }

    // fields
    #_features = ['name', 'rank', 'type'];

    #_default_sorts = [
        {
            descend: false,
            exclude: false,
            feature: this.#_features[0], // name
            asRegex: [],
            lookups: []
        },
        {
            descend: false,
            exclude: false,
            feature: this.#_features[1], // rank
            asRegex: [],    // unused
            lookups: []     // unused
        },
        {
            descend: false,
            exclude: false,
            feature: this.#_features[2], // type
            asRegex: [],
            lookups: [      // (function includes class)
                'boolean', 'bigint', 'class',  'function',
                'object',  'number', 'string', 'symbol',
                'undefined'
            ]
        },
    ];

    #_rank = 0;

    #_options = {
        defaultOnError: true,
        enumerables:    true,
        includeSymbols: true,
        inherited:      true,
        nonEnumerables: true,
        outputRootObj:  true,
        ownProperties:  true,
        ranks:          [-1],
        sortBy:         ['rank', 'name'],
        sorts:          this.#_get_sorts(this.#_default_sorts)
    };
}

const fetcherInstance = new PropertyFetcher();

var fetcherFlyWeight = {

    getOptions: (/** @type {boolean} */ defaultOrCurrent) => {

        return fetcherInstance.getOptions(defaultOrCurrent);
    },

    getPropertyInfo: (/** @type {any} */ obj) => {

        var info = [];
        try {
            info = fetcherInstance.getPropertyInfo(obj);
        } catch (e) {
            console.error('getPropertyInfo');
            throw e;
        }
        return info;
    },

    getPropertyInfoWithOptions: (
        /** @type {any}     */ obj,
        /** @type {any}     */ options,
        /** @type {boolean} */ useDefaultOnError) => {

        var info = [];
        try {
            if (fetcherInstance.setOptions(options, !!useDefaultOnError)) {
                info = fetcherInstance.getPropertyInfo(obj);
            }
        } catch (e) {
            console.error('getPropertyInfoWithOptions');
            throw e;
        }
        return info;
    },

    setOptions: (
        /** @type {any}     */ options,
        /** @type {boolean} */ useDefaultOnError) => {

        return fetcherInstance.setOptions(options, !!useDefaultOnError);
    },

    setPropertyNames: (
        /** @type {boolean}  */ exclude,
        /** @type {string[]} */ propertyNames) => {

        let result = true;
        try {
            fetcherInstance.setPropertyTypes(!!exclude, propertyNames);
        } catch {
            result = false;
        }
        return result;
    },

    setPropertyTypes: (
        /** @type {boolean}  */ exclude,
        /** @type {string[]} */ typeNames) => {

        let result = true;
        try {
            fetcherInstance.setPropertyTypes(!!exclude, typeNames);
        } catch {
            result = false;
        }
        return result;
    },
};

exports.Fetcher = fetcherFlyWeight;
