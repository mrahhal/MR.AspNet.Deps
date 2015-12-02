# MR.AspNet.Deps

[![Build status](https://img.shields.io/appveyor/ci/mrahhal/mr-aspnet-deps/master.svg)](https://ci.appveyor.com/project/mrahhal/mr-aspnet-deps)
[![Nuget version](https://img.shields.io/nuget/v/MR.AspNet.Deps.svg)](https://www.nuget.org/packages/MR.AspNet.Deps)
[![Nuget downloads](https://img.shields.io/nuget/dt/MR.AspNet.Deps.svg)](https://www.nuget.org/packages/MR.AspNet.Deps)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

An opinionated library that manages client side resources in AspNet 5 (js, css, scss, ...)

Check the gulp-aspnet-deps's [folder](src/gulp-aspnet-deps) for info about it.

## Note
This is a work in progress. When done, the process will consist of using a `bundle` tag helper + a gulp plugin to help with processing the bundles. All of this will depend on `deps.json`.

# Rationale
Managing client side resources is hell. There's no way other than an opinionated solution to manage this hell.
This is just what I see as the most appropriate solution.

# Overview
MR.AspNet.Deps depends on a file named `deps.json` in the root of your application (next to project.json).

## Structure of `deps.json`
```json
{
  "fonts": [
    "lib/bootstrap/dist/fonts/*",
    "lib/font-awesome/fonts/*"
  ],
  "js": [
    {
      "base": "lib/",
      "name": "vendor",
      "files": [
        "jquery/dist/jquery.js",
        "bootstrap/dist/js/bootstrap.js"
      ]
    },
    {
      "base": "_dev/js/",
      "name": "app",
      "files": [
        "main.js"
      ]
    }
  ],
  "css": [
    {
      "base": "lib/",
      "name": "vendor",
      "files": [
        "bootstrap/dist/css/bootstrap.css",
        "font-awesome/css/font-awesome.css"
      ]
    },
    {
      "base": "compiled/css/",
      "name": "app",
      "files": [
        "*"
      ]
    }
  ],
  "sass": [
    {
      "base": "_dev/sass/",
      "files": [
        "*"
      ]
    }
  ]
}
```

## Usage
Use the `bundle` tag helper in your layout file:
```html
<!-- For styles -->
<bundle kind="BundleKind.Style" name="vendor" />
<bundle kind="BundleKind.Style" name="app" />

<!-- For scripts -->
<bundle kind="BundleKind.Script" name="vendor" />
<bundle kind="BundleKind.Script" name="app" />
```

The bundle tag helper will generate the necessary tags based on the current environment.
If we're in development, script and link tags will be generated for the source files in a certain bundle. Otherwise, a single tag will be generated to a file named `[bundle name].[js|css]`. This file should be generated in your gulp build process.

This means your gulp build process should also depend on `deps.json` to find and process the bundles.
