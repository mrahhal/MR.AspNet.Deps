/* global require, module */

var merge = require('merge-stream');

var paths = {
	webroot: './wwwroot/'
};

function abs(path) {
	return paths.webroot + path;
}

function getFullFilesInBundle(bundle) {
	var base = bundle.base;
	var files = [];
	for (var j = 0; j < bundle.files.length; j++) {
		files.push(abs(base + bundle.files[j]));
	}
	return files;
}

function getExtFromKind(kind) {
	if (kind === 'script') {
		return '.js';
	} else if (kind === 'style') {
		return '.css';
	} else {
		throw 'Kind not supported.';
	}
}

var processBundles = function (bundles, kind, action) {
	return merge(bundles.map(function (bundle) {
		var name = bundle.name ? (bundle.name + getExtFromKind(kind)) : null;
		var files = getFullFilesInBundle(bundle);
		return action (name, files);
	}));
};

var processFonts = function (fonts, action) {
	var expanded = fonts.map(function (f) { return abs(f); });
	return action(expanded);
};

module.exports = {
	processFonts: processFonts,
	processBundles: processBundles
};
