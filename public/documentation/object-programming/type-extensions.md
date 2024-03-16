# Type Extensions

Type extensions allow us to extend the behavior of previously defined types by creating members for them.

```fsharp
module Extensions

type String with
//   ^^^^^^
// the type we will be extending
    member string.IsUpperCase() =
//         ^^^^^^
// this will be the instance of the string on which
// this member will be called
        string = string.ToUpper()

let uppercaseStringValue = "HELLO WORLD"
let lowercaseStringValue = "hello world"

uppercaseStringValue.IsUpperCase() // true
lowercaseStringValue.IsUpperCase() // false
```

This is called an optional type extension as we are exposing a type extension to a non-user defined type through a module/namespace that must be imported. This is in contrast to an instrinsic type extension that exists for a user-defined type and must be present in the same file.

```fsharp
module Domain

type Person = { FirstName: string; LastName: string }

type Person with
    member this.FullName =
        $"{person.FirstName} {person.LastName}"

let person = { FirstName = "John"; LastName = "Doe"; }
person.FullName // "John Doe"
```