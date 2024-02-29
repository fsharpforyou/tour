# Generic Types

Generic type parameters are placeholders for types that will be filled in by the caller. Generic types allow us to parameterize the types of values being used in bindings, function calls, or type definitions.

For example, you can have an `int list`, a `string list`, or a `float list`. Each of these will only contain values of their respective types. A `string list` cannot contain `float` values and a `float list` cannot contain `string` values. This is an example of generic type parameters in action.

You can define a generic type parameter using an apostrophe followed by the name of the type parameter.

```fsharp
type Data<'a> = { Value: 'a }
```

The `'a` in the above definition denotes a generic type parameter. If you wanted to define two generic type parameters you could use `'a` and `'b`. Sometimes, context-specific names are more appropriate choices, such as `'success` and `'error`, to model success and error types.

The `Value` property in the Data type can only contain a value of type `'a`. For `Data<string>` the `Value` property must contain a `string` value.

```fsharp
let data: Data<string> = { Value = "Hello, World!" }
let value: string = data.Value
```

You can also define generic type parameters in functions and pass them to your desired type. Here we can accept a value of our generic `Data` type, passing a generic type parameter to it in the process. 

```fsharp
let printData (data: Data<'a>) =
    printfn "%A" data.Value
```