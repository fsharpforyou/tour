# The Option Type

The built-in option type allows us to represent a value that may or may not exist in a composable manner.

Let's take a look at the definition of the option type

```fsharp
type Option<'a> =
    | Some of 'a
    | None
```

Here we can see that the option type has 2 potential cases:
 1. `Some` represent a present value
 2. `None` represents the absence of a value.

There are standard library functions that return `Option` values. One of these is `List.tryFind`, which tries to find the first value in a list matching a predicate.

```fsharp
type User = { Id: int; Name: string }

let tryFindUserById id users =
    List.tryFind (fun user -> user.Id = id) users
```

Options are composable using the `bind` and `map` functions.

The `map` function allows us to apply a transformation function the inner `Some` value of an option. The `map` function is defined as:

```fsharp
let map (f: 'a -> 'b) (option: 'a option) : 'b option =
    match option with
    | None -> None
    | Some value -> Some (f value)
```

Using the `map` function, we can transform a `User option` to a `string option` which represents the found user's name (if present).

```fsharp
let users = [ { Id = 1; Name = "John Doe" }; { Id = 2; Name = "Jane Doe" } ]

let optionalUsername =
    users
    |> tryFindUserById 1 // Some { Id = 1; Name = "John Doe" }
    |> Option.map (fun user -> user.Name) // Some "John Doe"

let optionalUsername' =
    users
    |> tryFindUserById 3 // None
    |> Option.map (fun user -> user.Name) // None
```

The `bind` function is similar, but the inner `Some` value is transformed into another `Option` value. It's defined as:

```fsharp
let bind (f: 'a -> 'b option) (option: 'a option) : 'b option =
    match option with
    | None -> None
    | Some value -> f value
```

With the bind function we can conditionally transform the `User` into a different type using `Option` values.

```fsharp
type User = { Id: int; Name: string; Age: int }
type AdultUser = { Name: string }

let userToAdultUser (user: User) : AdultUser option =
    if user.Age >= 18
    then Some { AdultUser.Name = user.Name }
    else None

let getAdultUser userId users =
    users
    |> tryFindUserById userId
    |> Option.bind userToAdultUser
```