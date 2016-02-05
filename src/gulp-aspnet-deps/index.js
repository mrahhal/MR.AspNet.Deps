/* global require, module */

var merge = require('merge-stream'),
	_ = require('lodash'),
	path = require('path');

var injected = {};

var overrides = {
	merge: function () {
		var fn = injected.merge || merge;
		fn.apply(null, arguments);
	}
};

var DEFAULTS = {
	webroot: 'wwwroot'
};

function join(/* */) {
	return path.normalize(path.join.apply(null, arguments));
}

var Helper = function (deps) {
	this.deps = deps;
	this.config = _.assign({}, DEFAULTS, this.deps.config);
};

Helper.prototype.getDefaults = function () {
	return _.extend({}, DEFAULTS);
};

Helper.prototype.makeAbsolutePath = function (val) {
	return join(this.config.webroot, val);
};

Helper.prototype.makeAbsoluteFiles = function (bundle) {
	if (!bundle.src) {
		return [];
	}

	var src = [];
	for (var j = 0; j < bundle.src.length; j++) {
		src.push(join(this.config.webroot, bundle.base, bundle.src[j]));
	}
	return src;
};

Helper.prototype.locateBundles = function (sectionName, bundleNames) {
	var self = this;

	if (!_.isString(sectionName)) {
		throw 'the sectionName should be a string refering to the name of the section';
	}

	var section = this.deps[sectionName];
	if (!section) {
		return;
	}

	var bundles = [];
	if (bundleNames) {
		for (var i = 0; i < section.length; i++) {
			var b = section[i];
			for (var j = 0; j < bundleNames.length; j++) {
				if (bundleNames[j] === b.name) {
					bundles.push(b);
					break;
				}
			};
		};
	} else {
		bundles = section;
	}

	return bundles;
};

Helper.prototype.process = function (sectionName, bundleNames, action) {
	var self = this;

	if (_.isFunction(bundleNames)) {
		action = bundleNames;
		bundleNames = undefined;
	}

	if (bundleNames && !_.isArray(bundleNames)) {
		var t = [];
		t.push(bundleNames);
		bundleNames = t;
	}

	var bundles = this.locateBundles(sectionName, bundleNames);

	var initials = bundles.map(function (bundle) {
		var b = self._processBundle(bundle, sectionName);
		return action(b);
	});

	var toMerge = [];
	for (var i = 0; i < initials.length; i++) {
		var t = initials[i];
		if (t && typeof t.pipe === 'function') {
			toMerge.push(t);
		}
	};

	return overrides.merge.apply(null, toMerge);
};

function normalizeBundle(bundle) {
	if (!bundle.base) {
		bundle.base = '';
	}
	if (!bundle.src) {
		bundle.src = [];
	}
}

function normalizeRefs(refs) {
	var ret = [];
	if (!_.isArray(refs)) {
		ret.push(refs);
	} else {
		ret = refs;
	}

	ret.forEach(function (r) {
		if (_.isString(r.bundles)) {
			r.bundles = [r.bundles];
		}
	});

	return ret;
}

Helper.prototype._processBundle = function (bundle, sectionName) {
	var self = this;

	bundle = _.assign({}, bundle);
	normalizeBundle(bundle);

	if (bundle.src && !_.isArray(bundle.src)) {
		throw 'src should be an array';
	}

	bundle.src = self.makeAbsoluteFiles(bundle);

	if (bundle.dest) {
		var dest = join(self.config.webroot, bundle.dest);
		bundle.dest = dest;
	}

	if (bundle.refs) {
		var refs = normalizeRefs(bundle.refs);
		for (var i = 0; i < refs.length; i++) {
			var ref = refs[i];
			var section = ref.section;
			var bundles = self.locateBundles(ref.section || sectionName, ref.bundles);

			for (var j = 0; j < bundles.length; j++) {
				var b = self._processBundle(bundles[j]);
				if (b.src) {
					Array.prototype.push.apply(bundle.src, b.src);
				}
			};
		};
	}

	return bundle;
};

module.exports = {
	override: injected,
	Helper: Helper,
	init: function (deps) {
		return new Helper(deps);
	}
};
