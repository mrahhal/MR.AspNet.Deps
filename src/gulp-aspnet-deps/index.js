/* global require, module */

var merge = require('merge-stream'),
	_ = require('lodash');

var injected = {};

var overrides = {
	merge: function () {
		var fn = injected.merge || merge;
		fn.apply(null, arguments);
	}
};

var DEFAULTS = {
	base: './wwwroot/'
};

var util = {
	ensureHasSlash: function (path) {
		if (!path) {
			return '';
		} else if (path != '' && path[path.length - 1] != '/') {
			path += '/';
		}
		return path;
	}
};

var Helper = function (config) {
	this.config = this._coarseConfig(config);
};

Helper.prototype.getDefaults = function () {
	return _.extend({}, DEFAULTS);
};

Helper.prototype._coarseConfig = function (config) {
	config.base = util.ensureHasSlash(config.base);
	return config;
};

Helper.prototype.makeAbsolutePath = function (path) {
	return this.config.base + path;
};

Helper.prototype.makeAbsoluteFiles = function (bundle) {
	if (!bundle.files) {
		return [];
	}

	var files = [],
		base = util.ensureHasSlash(bundle.base);

	for (var j = 0; j < bundle.files.length; j++) {
		files.push(this.makeAbsolutePath(base + bundle.files[j]));
	}
	return files;
};

Helper.prototype.process = function (bundles, action) {
	var self = this;

	if (!_.isArray(bundles)) {
		throw 'the bundles arg should be an array of bundle objects';
	}

	var initials = [];
	bundles.map(function (bundle) {
		if (!bundle.files || !_.isArray(bundle.files)) {
			throw 'all bundles should contain a "files" array';
		}
		var files = self.makeAbsoluteFiles(bundle);
		return action(bundle, files);
	});

	var toMerge = [];
	for (var i = 0; i < initials.length; i++) {
		var t = initials[i];
		if (t && typeof t.pipe === 'function') {
			toMerge.push(t);
		}
	};

	return overrides.merge(toMerge);
};

module.exports = {
	override: injected,
	util: util,
	Helper: Helper,
	init: function (config) {
		config = _.assign({}, DEFAULTS, config);
		return new Helper(config);
	}
};
