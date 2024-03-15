# Objects and Members

Objects encapsulate data and behavior into a single fundamental unit that can be operated on. The data and behavior of an object can be inherited from another object or be implemented based on a set of requirements provided by an interface. Objects can also have members, which are functions or values that can be used to access and operate on data within an object.

Let's start with the basics. First, you can define a type named `Person` that is created with `firstName` and `lastName` values.

```fsharp
type Person(firstName: string, lastName: string) = ...
```

The `firstName` and `lastName` parameters make up the objects constructor. To create an instance of this object, you need to pass these values to the constructor.

```fsharp
let person = Person("John", "Doe")
```

Next, you can create member values to expose the constructor parameters as public immutable values.

```fsharp
type Person(firstName: string, lastName: string) =
    member val FirstName = firstName with get
    member val LastName = lastName with get

let person = Person("John", "Doe")
person.FirstName // "John"
person.LastName // "Doe"
```

Members can also be functions that operate on the data enclosed within an object. Unlike the let-bound functions, they exist within an object and usually have a tuple as a single argument.

```fsharp
type Person(firstName: string, lastName: string) =
    member val FirstName = firstName with get
    member val LastName = lastName with get
    member this.Greet(greeting: string) = $"{greeting} {this.FirstName} {this.LastName}!"

let person = Person("John", "Doe")
let greeting = person.Greet("Hello") // "Hello John Doe!"
```

Note the usage of `this` in the above example. `this` is an arbitrary identifier that refers to the current instance of the object. The name `this` is arbitrary and can be anything, or even discarded with `_` although the `this` identifier is common as it's used in many other languages.

One thing to look out for when invoking members is type inference. The F# compiler cannot infer the types from member invocations alone and may need additional type annotations.

```fsharp
let test person =
    person.Greet("Hello") // <- compiler error
//         ^^^^^^^^^^^^^^
// The compiler can't determine the type of `person`
// because this member invocation is entirely arbitrary.

let test2 (person: Person) =
//         ^^^^^^^^^^^^^^
//         type annotation is required
    person.Greet("Hello") // <- this works
```

Members, unlike let-bound functions, have a unique ability to contain optional parameters with or without default values.

```fsharp
type Person(firstName: string, lastName: string) =
    member val FirstName = firstName with get
    member val LastName = lastName with get

    member this.Greet(?greeting: string) =
//                    ^
// notice that the parameter is prefixed with a ?
// which indicates that it's an optional parameter.
        let greeting = Option.defaultValue "Hello" greeting
        $"{greeting} {this.FirstName} {this.LastName}!"

let person = Person("John", "Doe")
person.Greet() // "Hello John Doe!"
person.Greet("Greetings") // "Greetings John Doe!"
```

One caveat to this is that if a member has multiple optional parameters, you may need to supply parameter names along with their values. This is because the parameter order by default will be sequential, but optional parameters can be skipped, so parameter names must be provided. 

```fsharp
type Person(firstName: string, lastName: string) =
    member val FirstName = firstName with get
    member val LastName = lastName with get

    member this.Greet(?greeting: string, ?punctuation: string) =
        let greeting = Option.defaultValue "Hello" greeting
        let punctuation = Option.defaultValue "!" punctuation
        $"{greeting} {this.FirstName} {this.LastName}{punctuation}"

let person = Person("John", "Doe")
person.Greet() // "Hello John Doe!"
person.Greet("Greetings") // "Greetings John Doe!"
person.Greet(punctuation = ".") // "Hello John Doe."
//           ^^^^^^^^^^^^^^^^^
// punctuation is the second parameter.
// without specifying the parameter name, the compiler will
// infer that the "." value would be the "greeting" parameter
// as its the first parameter.
```

To specify an optional parameters default value, you need to use the `[<Optional>]` and `[<DefaultParameterValue("...")>]` annotations instead of the `?parameter` syntax.

```fsharp
open System.Runtime.InteropServices
// this import is required to use the annotation

type Person(firstName: string, lastName: string) =
    member val FirstName = firstName with get
    member val LastName = lastName with get

    member this.Greet([<Optional; DefaultParameterValue("Hello")>] greeting: string) =
        $"{greeting} {this.FirstName} {this.LastName}!"

let person = Person("John", "Doe")
person.Greet() // "Hello John Doe!"
person.Greet("Greetings") // "Greetings John Doe!"
```