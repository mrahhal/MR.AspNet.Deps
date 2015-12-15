/* global describe, it */

var should = require('should'),
	builder = require('../index.js');

describe('builder', function () {
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
			helper.config.should.have.property('base', 'some/');
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

		it('should throw if no files exist in a bundle', function () {
			should.throws(function () {
				helper.process([{
						name: "app"
					}], function () { });
			});
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
					files: []
				}, {
					name: 'app',
					files: []
				}], function (bundle, files) {
				processedBundles.push(bundle);
			});
			processedBundles.length.should.be.exactly(2);
		});

		it('should maintain values in the bundle', function () {
			helper.process([{
					foo: 'bar',
					files: []
				}], function (bundle) {
				bundle.should.have.property('foo', 'bar');
			});
		});

		it('should expand files properly when no base is provided', function () {
			helper.process([{
					name: 'app',
					files: [
						'foo.js'
					]
				}], function (bundle, files) {
				files[0].should.be.exactly(helper.getDefaults().base + 'foo.js');
			});
		});

		it('should expand files properly when a base is provided', function () {
			helper = builder.init({ base: './foo/' });
			helper.process([{
					name: 'app',
					files: [
						'foo.js'
					]
				}], function (bundle, files) {
				files[0].should.be.exactly('./foo/foo.js');
			});
		});

		it('should expand files properly when a base is provided in the bundle', function () {
			helper = builder.init({ base: './foo/' });
			helper.process([{
					name: 'app',
					base: 'bar',
					files: [
						'foo.js'
					]
				}], function (bundle, files) {
				files[0].should.be.exactly('./foo/bar/foo.js');
			});
		});
	});
});
