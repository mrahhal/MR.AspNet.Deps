# gulp-aspnet-deps

[![Travis](https://img.shields.io/travis/mrahhal/MR.AspNet.Deps.svg)](https://travis-ci.org/mrahhal/MR.AspNet.Deps)
[![npm](https://img.shields.io/npm/v/gulp-aspnet-deps.svg)](https://www.npmjs.com/package/gulp-aspnet-deps)
[![npm](https://img.shields.io/npm/dt/gulp-aspnet-deps.svg)](https://www.npmjs.com/package/gulp-aspnet-deps)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

A gulp plugin that helps with processing client side resources when working with [`MR.AspNet.Deps`](https://github.com/mrahhal/MR.AspNet.Deps).

## Usage
```js
var deps = require('./deps.json');
var depsBuilder = require('gulp-aspnet-deps');

var depsHelper = depsBuilder.init({
    base: './wwwroot/',
});

// An example with minification of js.
gulp.task('min:js', function () {
    return depsHelper.process(deps.js, function (bundle) {
        return gulp.src(bundle.files)
            .pipe(concat(path.join(bundle.target, bundle.name + '.js')))
            .pipe(uglify())
            .pipe(gulp.dest('.'));
    });
});
```

`depsHelper.process` takes a section from the `deps.json` file and a function that will be called with a bundle object and the files with their full paths so you can directly call `gulp.src` on them.

The `bundle` object you get in the callback of the `process` function will have the `target` and the `files` properties resolved and normalized relative to the `base` property you provided when calling `depsBuilder.init` so you can use them in your gulpfile.

### Utils you can use
- `depsHelper.makeAbsolutePath`: takes a path and returns the absolute path relative to the `base` property you provided when calling `depsBuilder.init`.
- `depsHelper.makeAbsoluteFiles`: takes a bundle and returns the absolute files relative to the `base` property you provided when calling `depsBuilder.init`.
