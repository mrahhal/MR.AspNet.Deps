/* global describe, it */

var should = require('should'),
	builder = require('../index.js'),
	path = require('path'),
	_ = require('lodash');

function join(/* */) {
	return path.normalize(path.join.apply(null, arguments));
}

describe('deps', function () {
	it('should export interface', function () {
		builder.should.have.property('Helper');
		builder.should.have.property('init');
	});

	describe('init', function () {
		it('should throw if deps is undefined', function () {
			should.throws(function () {
				builder.init();
			});
		});

		it('should instantiate a Helper', function () {
			var helper = builder.init({});
			helper.should.be.instanceof(builder.Helper);
		});

		it('should correctly use the provided config', function () {
			var helper = builder.init({ config: { base: 'some' } });
			helper.config.should.have.property('base', 'some');
		});

		it('should correctly use the default config', function () {
			var helper = builder.init({});
			helper.config.should.have.property('base', helper.getDefaults().base);
		});
	});

	describe('helper', function () {
		function resetOverrides() {
			for (var prop in builder.override) {
				if (builder.override.hasOwnProperty(prop)) {
					delete builder.override[prop];
				}
			}
		}

		beforeEach(function () {
			builder.override.merge = function (values) {
				return values;
			};
		});

		afterEach(function () {
			resetOverrides();
		});

		function process(section, bundleName, config, action) {
			if (_.isFunction(config)) {
				action = config;
				config = {};
			}
			if (_.isFunction(bundleName)) {
				action = bundleName;
				bundleName = undefined;
			}
			if (_.isPlainObject(bundleName)) {
				config = bundleName;
				bundleName = undefined;
			}
			var helper = builder.init({
				config: config,
				section: section
			});
			helper.process('section', bundleName, function (bundle) {
				return action(bundle, helper);
			});
		}

		it('should process all bundles', function () {
			var processedBundles = [];
			process([{
					name: 'vendor',
					src: []
				}, {
					name: 'app',
					src: []
				}], function (bundle) {
				processedBundles.push(bundle);
			});
			processedBundles.length.should.be.exactly(2);
		});

		it('should maintain values in the bundle', function () {
			process([{
					foo: 'bar',
					src: []
				}], function (bundle) {
				bundle.should.have.property('foo', 'bar');
			});
		});

		it('should expand src properly when no base is provided', function () {
			process([{
					name: 'app',
					src: [
						'foo.js'
					]
				}], function (bundle, helper) {
				bundle.src[0].should.be.exactly(join(helper.getDefaults().base , 'foo.js'));
			});
		});

		it('should expand src properly when a base is provided', function () {
			process([{
					name: 'app',
					src: [
						'foo.js'
					]
				}], { base: './foo/' }, function (bundle) {
				bundle.src[0].should.be.exactly(join('foo', 'foo.js'));
			});
		});

		it('should expand src properly when a base is provided in the bundle', function () {
			process([{
					name: 'app',
					base: 'bar',
					src: [
						'foo.js'
					]
				}], { base: './foo/' }, function (bundle) {
				bundle.src[0].should.be.exactly(join('foo', 'bar', 'foo.js'));
			});
		});

		it('should normalize paths', function () {
			process([{
					name: 'app',
					base: 'bar/some/',
					dest: 'baz',
					src: [
						'foo.js'
					]
				}], { base: './foo/' }, function (bundle) {
				bundle.dest.should.be.exactly(path.normalize(path.join('foo', 'baz')));
			});
		});

		it('should expand dest properly if provided', function () {
			process([{
					name: 'app',
					base: 'bar/',
					dest: '../baz',
					src: [
						'foo.js'
					]
				}], { base: './foo/some/' }, function (bundle) {
				bundle.dest.should.be.exactly(join('foo', 'baz'));
			});
		});

		it('should do destructive edits on a bundle copy instead of the original bundle', function () {
			var section = [{
					name: 'app',
					base: 'bar/',
					dest: '../baz',
					src: [
						'foo.js'
					]
				}];
			process(section, { base: './foo/some/' }, function (bundle) {
				bundle.foo = 42;
			});
			section[0].should.not.have.ownProperty('foo');
		});

		it('should process the bundle if its name is specified', function () {
			var section = [{
					name: 'app'
				}, {
					name: 'vendor'
				}];
			process(section, 'app', function (bundle) {
				bundle.name.should.be.exactly('app');
			});
		});
	});
});
