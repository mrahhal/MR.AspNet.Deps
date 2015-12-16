# MR.AspNet.Deps

[![Build status](https://img.shields.io/appveyor/ci/mrahhal/mr-aspnet-deps/master.svg)](https://ci.appveyor.com/project/mrahhal/mr-aspnet-deps)
[![Nuget version](https://img.shields.io/nuget/v/MR.AspNet.Deps.svg)](https://www.nuget.org/packages/MR.AspNet.Deps)
[![Nuget downloads](https://img.shields.io/nuget/dt/MR.AspNet.Deps.svg)](https://www.nuget.org/packages/MR.AspNet.Deps)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://opensource.org/licenses/MIT)

An opinionated library that manages client side resources in AspNet5 (js, css, scss, ...)

Check the gulp-aspnet-deps's [folder](src/gulp-aspnet-deps) for info about it.

# Rationale
Managing client side resources is hell. There's no way other than an opinionated solution to manage this hell.

This is all about reuse, as we don't want to specify our resources more than one time. This is not necessarily about making startup configuration small, as this is always needed. This is about long time sane maintenance of resources.

# Samples
For samples of how to work with both the `bundle` tag helper and the gulp plugin check the [samples](samples) folder.

# Overview
MR.AspNet.Deps depends on a file named `deps.json` in the root of your application (next to project.json).
The process consists of using a `bundle` tag helper + a gulp plugin to help with processing the bundles. All of this will depend on `deps.json`.

Add Deps to the service collection:
```c#
services.AddDeps();
```

Add the tag helpers in `_ViewImports.cshtml`:
```
@addTagHelper "*, MR.AspNet.Deps"
```

## Structure of `deps.json`
```json
{
  "js": [
    {
      "name": "vendor",
      "base": "lib/",
      "files": [
        "*"
      ]
    }
  ],
  "css": [
  ],
  "others": [
  ]
}
```

`MR.AspNet.Deps` is concerned only with the `js` and the `css` sections. But you can (and should) use `deps.json` for all of your resources (such as sass files).
You can process the bundles using the gulp plugin [`gulp-aspnet-deps`](src/gulp-aspnet-deps).

In each bundle only the `files` prop is required.

### An example of a `deps.json`
```json
{
  "copy": [
    {
      "target": "fonts/",
      "files": [
        "lib/bootstrap-sass-official/assets/fonts/bootstrap/*",
        "lib/font-awesome/fonts/*"
      ]
    }
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
