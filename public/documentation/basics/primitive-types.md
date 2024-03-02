# Primitive Types

The fundamental types used in most F# programs are `int`, `float`, `bool`, `char`, `string`, and `unit`.
These types are called primitives, as they are basic data types that contain simple values and are the foundation for other, more complex types.

What do these primitive types represent?

- `int` represents a 32-bit numerical value, that is, a number without a decimal point.
- `float` represents a 64-bit double-precision floating-point numerical value, that is, a number _with_ a decimal point.
- `char` represents a Unicode character value, like an individual letter or emoji.
- `string` represents a sequence of `char` values, that is, a piece of text.
- `bool` represents a `true` or `false` value, often used in conditional logic.
- `unit`, when passed to a function, means that function has no arguments. When returned from a function, it indicates that the function has no useful return value. Often used when a function e.g. prints to the screen and does nothing else.

```fsharp
10              // int
10.0            // float
'a'             // char
"Hello, World!" // string
true            // bool
()              // unit
```
