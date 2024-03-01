# Type Abbreviations

Using type abbreviations, you can define an alias for any existing type. These are often used to create a shorter and reusable name for an existing type or function signature.

```fsharp
type Logger = string -> unit
```

Because the `Logger` type is just an abbreviation for the function signature `string -> unit` you can use the two interchangeably.

```fsharp
let exclaim (logger: Logger) (value: string) = logger (sprintf "%s!!!" value)

let logger: Logger = printfn "%s"
exclaim logger "Hello"
```