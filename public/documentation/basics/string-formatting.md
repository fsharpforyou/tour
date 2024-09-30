# String Formatting

String formatting is the process of integrating additional values into string literals.

```fsharp
let name = "John Doe"
let nameDescription = sprintf "Your name is %s" name
printfn "%s" nameDescription
```

Here, we specify that the string format contains a single string value, indicated by the `%s` format specifier. 

Other common format specifiers include:  
`%b` for boolean values.   
`%d` for integer values.  
`%f` for floating point values.  
`%O` which uses the values string representation (calls `value.ToString()`).  
`%A` which uses a structured plain-text representation.

The `sprintf`, `printf`, and `printfn` all take format specifiers like the ones above. The `sprintf` function will create a string value from a template while the `printf` and `printfn` functions will print values to the standard output, the latter adding a newline at the end.

```fsharp
printfn "Hello, %s!" name
```

Another method is to use interpolated strings which allow you to bake the values directly into a string literal.

```fsharp
let name = "John Doe"
printfn $"Your name is {name}"
```

These interpolated strings can also be type checked by providing a format before the template. This will result in a compiler error if the type of the value doesn't match the format specifier.

```fsharp
printfn $"Your name is %s{name}" // this works.
printfn $"Your name is %b{name}" // compiler error. %b = boolean
```