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
	base: 'wwwroot'
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

Helper.prototype._normalizeBundle = function (bundle) {
	if (!bundle.base) {
		bundle.base = '';
	}
	if (!bundle.src) {
		bundle.src = [];
	}
};

Helper.prototype.makeAbsolutePath = function (val) {
	return join(this.config.base, val);
};

Helper.prototype.makeAbsoluteFiles = function (bundle) {
	if (!bundle.src) {
		return [];
	}

	var src = [];
	for (var j = 0; j < bundle.src.length; j++) {
		src.push(join(this.config.base, bundle.base, bundle.src[j]));
	}
	return src;
};

Helper.prototype.process = function (sectionName, bundleName, action) {
	var self = this;

	if (_.isFunction(bundleName)) {
		action = bundleName;
		bundleName = undefined;
	}

	if (!_.isString(sectionName)) {
		throw 'the sectionName should be a string refering to the name of the section';
	}

	var section = this.deps[sectionName];
	if (!section) {
		return;
	}

	var bundles = [];
	if (bundleName) {
		for (var i = 0; i < section.length; i++) {
			var b = section[i];
			if (bundleName === b.name) {
				bundles.push(b);
				break;
			}
		};
	} else {
		bundles = section;
	}

	var initials = bundles.map(function (bundle) {
		self._normalizeBundle(bundle);
		bundle = _.assign({}, bundle);

		if (bundle.src && !_.isArray(bundle.src)) {
			throw 'src should be an array';
		}

		if (bundle.dest) {
			var dest = join(self.config.base, bundle.dest);
			bundle.dest = dest;
		}

		var src = self.makeAbsoluteFiles(bundle);
		bundle.src = src;
		return action(bundle);
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

module.exports = {
	override: injected,
	Helper: Helper,
	init: function (deps) {
		return new Helper(deps);
	}
};
