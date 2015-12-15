/* global describe, it */

var should = require('should'),
	builder = require('../index.js');

describe('util', function () {
	var util = builder.util;

	describe('ensureHasSlash', function () {
		it('should handle undefined path', function () {
			var result = util.ensureHasSlash();
			result.should.be.exactly('');
		});

		it('should handle empty path', function () {
			var result = util.ensureHasSlash('');
			result.should.be.exactly('');
		});

		it('should not append slash if a slash exists', function () {
			var result = util.ensureHasSlash('some/');
			result.should.be.exactly('some/');
		});

		it('should append slash', function () {
			var result = util.ensureHasSlash('some');
			result.should.be.exactly('some/');
		});
	});
});
