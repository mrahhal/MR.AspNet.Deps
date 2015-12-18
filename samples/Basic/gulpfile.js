/// <binding Clean='clean' ProjectOpened='compile:sass' />
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
	deps = require('gulp-aspnet-deps').init(require('./deps.json')),
	path = require('path');

function abs() {
	return deps.makeAbsolutePath.apply(deps, arguments);
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
	return deps.process('js', function (bundle) {
		return gulp.src(bundle.src)
			.pipe(concat(path.join(bundle.dest, bundle.name + '.js')))
			.pipe(uglify())
			.pipe(gulp.dest('.'));
	});
});

gulp.task('watch:sass', function () {
	var files = [];
	deps.process('sass', function (bundle) {
		Array.prototype.push.apply(files, bundle.src);
	});

	//var files = [];
	//for (var i = 0; i < deps.sass.length; i++) {
	//	var t = depsHelper_____________.makeAbsoluteFiles(deps.sass[i]);
	//	for (var j = 0; j < t.length; j++) {
	//		files.push(t[j]);
	//	}
	//}
	watch(files, function () {
		gulp.start('compile:sass');
	});
});

gulp.task('compile:sass', function () {
	return deps.process('sass', function (bundle) {
		return gulp.src(bundle.src)
			.pipe(sourcemaps.init())
			.pipe(sass().on('error', sass.logError))
			.pipe(sourcemaps.write())
			.pipe(gulp.dest(bundle.dest));
	});
});

gulp.task('min:css', function () {
	return deps.process('css', function (bundle) {
		return gulp.src(bundle.src)
			.pipe(sass())
			.pipe(concat(path.join(bundle.dest, bundle.name + '.css')))
			.pipe(cssmin())
			.pipe(gulp.dest('.'));
	});
});

gulp.task('copy', function () {
	return deps.process('copy', function (bundle) {
		return gulp.src(bundle.src)
			.pipe(gulp.dest(bundle.dest));
	});
});

gulp.task("default", ["min:js", "min:css", "copy"]);
