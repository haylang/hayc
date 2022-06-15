# The Hay Compiler - hayc
![Custom badge](https://img.shields.io/badge/haylang-compiler-blue?style=plastic)
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/haylang/hayc/.NET?style=plastic)
![GitHub License](https://img.shields.io/github/license/haylang/hayc?style=plastic)
![GitHub top language](https://img.shields.io/github/languages/top/haylang/hayc?style=plastic)


Hay is a simple weak procedural statically-typed language. It's heavily inspired by C, but aims to fix some its peculiarities that I find annoying, for example having multiple syntax options for one operation.

**This project is under current development.**

### `Hello world` in Hay
```cpp
include std;

namespace app;

function hello_world() : void {
    std::out().write("Hello, world!\n");
}
```

### Compiling
At the moment the project has no dependencies that need to be installed. It is being developed with .NET 6.

### Usage
To see command usages, run `hayc`.

To compile a project, run `hayc build <path>`.