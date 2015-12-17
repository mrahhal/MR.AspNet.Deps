/// <binding Clean='clean' />
"use strict";

var gulp = require("gulp"),
	rimraf = require("rimraf"),
	concat = require("gulp-concat"),
	cssmin = require("gulp-cssmin"),
	uglify = require("gulp-uglify"),
	rimraf = require('gulp-rimraf'),
	sass = require('gulp-sass'),
	watch = require('gulp-watch'),
	sourcemaps = require('gulp-sourcemaps'),
	deps = require('./deps.json'),
	depsBuilder = require('gulp-aspnet-deps'),
	path = require('path');

var paths = {
	webroot: "./wwwroot/"
};

var depsHelper = depsBuilder.init({
	base: paths.webroot
});

function abs() {
	return depsHelper.makeAbsolutePath.apply(depsHelper, arguments);
}

gulp.task('clean:js', function () {
	return gulp.src(abs('js/*'))
		.pipe(rimraf());
});

gulp.task('clean:css', function () {
	return gulp.src([abs('css/*'), abs('compiled/*')])
		.pipe(rimraf());
});

gulp.task("clean", ["clean:js", "clean:css"]);

gulp.task('min:js', function () {
	return depsHelper.process(deps.js, function (bundle) {
		return gulp.src(bundle.files)
			.pipe(concat(path.join(bundle.target, bundle.name + '.js')))
			.pipe(uglify())
			.pipe(gulp.dest('.'));
	});
});

gulp.task('watch:sass', function () {
	var files = [];
	for (var i = 0; i < deps.sass.length; i++) {
		var t = depsHelper.makeAbsoluteFiles(deps.sass[i]);
		for (var j = 0; j < t.length; j++) {
			files.push(t[j]);
		}
	}
	watch(files, function () {
		gulp.start('compile:sass');
	});
});

gulp.task('compile:sass', function () {
	return depsHelper.process(deps.sass, function (bundle) {
		return gulp.src(bundle.files)
			.pipe(sourcemaps.init())
			.pipe(sass().on('error', sass.logError))
			.pipe(sourcemaps.write())
			.pipe(gulp.dest(bundle.target));
	});
});

gulp.task('min:css', ['compile:sass'], function () {
	return depsHelper.process(deps.css, function (bundle) {
		return gulp.src(bundle.files)
			.pipe(concat(path.join(bundle.target, bundle.name + '.css')))
			.pipe(cssmin())
			.pipe(gulp.dest('.'));
	});
});

gulp.task('copy', function () {
	return depsHelper.process(deps.copy, function (bundle) {
		return gulp.src(bundle.files)
			.pipe(gulp.dest(bundle.target));
	});
});

gulp.task("default", ["min:js", "min:css", "copy"]);
