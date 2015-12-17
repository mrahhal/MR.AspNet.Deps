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

var Helper = function (config) {
	this.config = _.assign({}, DEFAULTS, config);
};

Helper.prototype.getDefaults = function () {
	return _.extend({}, DEFAULTS);
};

Helper.prototype._normalizeBundle = function (bundle) {
	if (!bundle.base) {
		bundle.base = '';
	}
};

Helper.prototype.makeAbsolutePath = function (val) {
	return join(this.config.base, val);
};

Helper.prototype.makeAbsoluteFiles = function (bundle) {
	if (!bundle.files) {
		return [];
	}

	var files = [];
	for (var j = 0; j < bundle.files.length; j++) {
		files.push(join(this.config.base, bundle.base, bundle.files[j]));
	}
	return files;
};

Helper.prototype.process = function (bundles, action) {
	var self = this;

	if (!_.isArray(bundles)) {
		throw 'the bundles arg should be an array of bundle objects';
	}

	var initials = bundles.map(function (bundle) {
		self._normalizeBundle(bundle);

		if (!bundle.files || !_.isArray(bundle.files)) {
			throw 'all bundles should contain a "files" array';
		}

		if (bundle.target) {
			var target = join(self.config.base, bundle.target);
			bundle.target = target;
		}

		var files = self.makeAbsoluteFiles(bundle);
		bundle.files = files;
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
	init: function (config) {
		return new Helper(config);
	}
};
