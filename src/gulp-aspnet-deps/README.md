# gulp-aspnet-deps

[![Travis](https://img.shields.io/travis/mrahhal/MR.AspNet.Deps/master.svg)](https://travis-ci.org/mrahhal/MR.AspNet.Deps)
[![npm](https://img.shields.io/npm/v/gulp-aspnet-deps.svg)](https://www.npmjs.com/package/gulp-aspnet-deps)
[![npm](https://img.shields.io/npm/dt/gulp-aspnet-deps.svg)](https://www.npmjs.com/package/gulp-aspnet-deps)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

A gulp plugin that helps with processing client side resources when working with [`MR.AspNet.Deps`](https://github.com/mrahhal/MR.AspNet.Deps).

## Usage
```js
var deps = require('gulp-aspnet-deps').init(require('./deps.json'));

// An example with minification of js.
gulp.task('min:js', function () {
    return deps.process('js', function (bundle) {
        return gulp.src(bundle.src)
            .pipe(concat(path.join(bundle.dest, bundle.name + '.js')))
            .pipe(uglify())
            .pipe(gulp.dest('.'));
    });
});
```

`deps.process` takes a section from the `deps.json` file and a function that will be called with a bundle object and the files with their full paths so you can directly call `gulp.src` on them.

The `bundle` object you get in the callback of the `process` function will have the `dest` and the `src` properties resolved and normalized relative to `config.base` in `deps.json` so you can use them in your gulpfile.

### Utils you can use
- `deps.makeAbsolutePath`: takes a path and returns the absolute path relative to `config.base`.
- `deps.makeAbsoluteFiles`: takes a bundle and returns the absolute files relative to `config.base`.
