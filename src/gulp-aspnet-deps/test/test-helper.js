/* global describe, it */

var should = require('should'),
	builder = require('../index.js'),
	path = require('path');

function join(/* */) {
	return path.normalize(path.join.apply(null, arguments));
}

describe('deps', function () {
	it('should export interface', function () {
		builder.should.have.property('Helper');
		builder.should.have.property('init');
	});

	describe('init', function () {
		it('should handle invalid config', function () {
			should.doesNotThrow(function () {
				should.exist(builder.init());
			});
			should.doesNotThrow(function () {
				should.exist(builder.init({}));
			});
		});

		it('should instantiate a Helper', function () {
			var helper = builder.init();
			helper.should.be.instanceof(builder.Helper);
		});

		it('should correctly use the provided config', function () {
			var helper = builder.init({ base: 'some' });
			helper.config.should.have.property('base', 'some');
		});

		it('should correctly use the default config', function () {
			var helper = builder.init();
			helper.config.should.have.property('base', helper.getDefaults().base);
		});
	});

	describe('helper', function () {
		var helper = builder.init();

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
			helper = builder.init();
		});

		afterEach(function () {
			resetOverrides();
		});

		it('should throw if the bundles arg is invalid', function () {
			should.throws(function () {
				helper.process({}, function () { });
			});
			should.throws(function () {
				helper.process(null, function () { });
			});
			should.throws(function () {
				helper.process(function () { }, function () { });
			});
		});

		it('should process all bundles', function () {
			var processedBundles = [];
			helper.process([{
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
			helper.process([{
					foo: 'bar',
					src: []
				}], function (bundle) {
				bundle.should.have.property('foo', 'bar');
			});
		});

		it('should expand src properly when no base is provided', function () {
			helper.process([{
					name: 'app',
					src: [
						'foo.js'
					]
				}], function (bundle) {
				bundle.src[0].should.be.exactly(join(helper.getDefaults().base , 'foo.js'));
			});
		});

		it('should expand src properly when a base is provided', function () {
			helper = builder.init({ base: './foo/' });
			helper.process([{
					name: 'app',
					src: [
						'foo.js'
					]
				}], function (bundle) {
				bundle.src[0].should.be.exactly(join('foo', 'foo.js'));
			});
		});

		it('should expand src properly when a base is provided in the bundle', function () {
			helper = builder.init({ base: './foo/' });
			helper.process([{
					name: 'app',
					base: 'bar',
					src: [
						'foo.js'
					]
				}], function (bundle) {
				bundle.src[0].should.be.exactly(join('foo', 'bar', 'foo.js'));
			});
		});

		it('should normalize paths', function () {
			helper = builder.init({ base: './foo/' });
			helper.process([{
					name: 'app',
					base: 'bar/some/',
					dest: 'baz',
					src: [
						'foo.js'
					]
				}], function (bundle) {
				bundle.dest.should.be.exactly(path.normalize(path.join('foo', 'baz')));
			});
		});

		it('should expand dest properly if provided', function () {
			helper = builder.init({ base: './foo/some/' });
			helper.process([{
					name: 'app',
					base: 'bar/',
					dest: '../baz',
					src: [
						'foo.js'
					]
				}], function (bundle) {
				bundle.dest.should.be.exactly(join('foo', 'baz'));
			});
		});

		it('should do destructive edits on a bundle copy instead of the original bundle', function () {
			helper = builder.init({ base: './foo/some/' });
			var section = [{
					name: 'app',
					base: 'bar/',
					dest: '../baz',
					src: [
						'foo.js'
					]
				}];
			helper.process(section, function (bundle) {
				bundle.foo = 42;
			});
			section[0].should.not.have.ownProperty('foo');
		});
	});
});
