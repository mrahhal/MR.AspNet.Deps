/* global describe, it */

var should = require('should'),
	rewire = require('rewire'),
	builder = rewire('../index.js'),
	path = require('path'),
	_ = require('lodash');

builder.__set__('merge', function (values) {
	return values;
});

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
			var helper = builder.init({ config: { webroot: 'some' } });
			helper.config.should.have.property('webroot', 'some');
		});

		it('should correctly use the default config', function () {
			var helper = builder.init({});
			helper.config.should.have.property('webroot', helper.getDefaults().webroot);
		});
	});

	describe('helper', function () {
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

		it('should handle negation', function () {
			var deps = {
				sec1: [{
						name: 'app',
						src: ['!src/*.spec.js']
					}]
			};

			var helper = builder.init(deps);

			helper.process('sec1', 'app', function (bundle) {
				bundle.src[0].should.be.exactly('!' + join('wwwroot', 'src', '*.spec.js'));
			});
		});

		it('should expand src properly when no webroot is provided', function () {
			process([{
					name: 'app',
					src: ['foo.js']
				}], function (bundle, helper) {
				bundle.src[0].should.be.exactly(join(helper.getDefaults().webroot , 'foo.js'));
			});
		});

		it('should expand src properly when a base is provided', function () {
			process([{
					name: 'app',
					src: ['foo.js']
				}], { webroot: './foo/' }, function (bundle) {
				bundle.src[0].should.be.exactly(join('foo', 'foo.js'));
			});
		});

		it('should expand src properly when a base is provided in the bundle', function () {
			process([{
					name: 'app',
					base: 'bar',
					src: ['foo.js']
				}], { webroot: './foo/' }, function (bundle) {
				bundle.src[0].should.be.exactly(join('foo', 'bar', 'foo.js'));
			});
		});

		it('should normalize paths', function () {
			process([{
					name: 'app',
					base: 'bar/some/',
					dest: 'baz',
					src: ['foo.js']
				}], { webroot: './foo/' }, function (bundle) {
				bundle.dest.should.be.exactly(path.normalize(path.join('foo', 'baz')));
			});
		});

		it('should expand dest properly if provided', function () {
			process([{
					name: 'app',
					base: 'bar/',
					dest: '../baz',
					src: ['foo.js']
				}], { webroot: './foo/some/' }, function (bundle) {
				bundle.dest.should.be.exactly(join('foo', 'baz'));
			});
		});

		it('should do destructive edits on a bundle copy instead of the original bundle', function () {
			var section = [{
					name: 'app',
					base: 'bar/',
					dest: '../baz',
					src: ['foo.js']
				}];
			process(section, { webroot: './foo/some/' }, function (bundle) {
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

		it('should process the specified bundle names', function () {
			var count = 0;
			var section = [{
					name: 'app'
				}, {
					name: 'vendor'
				}, {
					name: 'other'
				}];
			process(section, ['app', 'other'], function (bundle) {
				count++;
			});
			count.should.be.exactly(2);
		});

		describe('refs', function () {
			it('should process refs', function () {
				var deps = {
					sec1: [{
							name: 'app',
							refs: {
								section: 'sec2',
								bundles: 'app'
							},
							src: ['bar.js']
						}],
					sec2: [{
							base: 'some',
							name: 'app',
							src: ['foo.js']
						}]
				};

				var helper = builder.init(deps);

				helper.process('sec1', 'app', function (bundle) {
					bundle.src.length.should.be.exactly(2);
					bundle.src[0].should.be.exactly(join('wwwroot', 'bar.js'));
					bundle.src[1].should.be.exactly(join('wwwroot', 'some', 'foo.js'));
				});
			});

			it('should process refs even without src', function () {
				var deps = {
					sec1: [{
							name: 'app',
							refs: {
								section: 'sec2',
								bundles: 'app'
							}
						}],
					sec2: [{
							base: 'some',
							name: 'app',
							src: ['foo.js']
						}]
				};

				var helper = builder.init(deps);

				helper.process('sec1', 'app', function (bundle) {
					bundle.src.length.should.be.exactly(1);
					bundle.src[0].should.be.exactly(join('wwwroot', 'some', 'foo.js'));
				});
			});

			it('should process multi refs', function () {
				var deps = {
					sec1: [{
							name: 'app',
							refs: [{
									section: 'sec2',
									bundles: 'app'
								}, {
									section: 'sec3',
									bundles: 'app'
								}]
						}],
					sec2: [{
							base: 'some',
							name: 'app',
							src: ['foo.js']
						}],
					sec3: [{
							name: 'app',
							src: ['baz.js']
						}]
				};

				var helper = builder.init(deps);

				helper.process('sec1', 'app', function (bundle) {
					bundle.src.length.should.be.exactly(2);
					bundle.src[0].should.be.exactly(join('wwwroot', 'some', 'foo.js'));
					bundle.src[1].should.be.exactly(join('wwwroot', 'baz.js'));
				});
			});

			it('should use the same section if non is specified', function () {
				var deps = {
					sec1: [{
							name: 'app',
							refs: {
								bundles: 'vendor'
							}
						}, {
							name: 'vendor',
							src: ['foo.js']
						}]
				};

				var helper = builder.init(deps);

				helper.process('sec1', 'app', function (bundle) {
					bundle.src.length.should.be.exactly(1);
					bundle.src[0].should.be.exactly(join('wwwroot', 'foo.js'));
				});
			});
		});
	});
});
