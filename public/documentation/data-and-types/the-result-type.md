# The Result Type

The built-in result type allows us to model success and failure states in a composable manner.

Let's take a look at the definition of the result type:

```fsharp
type Result<'ok, 'error> =
    | Ok of 'ok
    | Error of 'error
```

Here, we can see that the result type is a discriminated union with 2 cases:
1. `Ok` to represent success states.
2. `Error` to represent failure states.

Let's use the result type to create a `safeDivide` function that returns either the result after division or an error message.

```fsharp
let safeDivide x y = 
    if y = 0 then
        Error "Can't divide by 0"
    else
        Ok (x / y)

safeDivide 4 2 // Ok 2
safeDivide 5 0 // Error "Can't divide by 0"
```

It's quite common to model possible errors with a discriminated union, creating a predefined set of possible errors for the given operation.

```fsharp
type AccountCreationDetails = { Username: string; EmailAddress: string }

type CreateAccountError =
    | UsernameNotAvailable
    | EmailAddressNotAvailable

let createAccount details = ...
```

Like the `Option` type, you can compose and transform `Result` types using the `map` and `bind` operations.

```fsharp
(*
    ensureUsernameIsAvailable : Account list -> Account -> Result<Account, CreateAccountError>
    ensureEmailIsAvailable : Account list -> Account -> Result<Account, CreateAccountError>
*)

type Account = { Username: string; EmailAddress: string }

type CreateAccountError =
    | UsernameNotAvailable
    | EmailAddressNotAvailable

let createAccount existingAccounts account =
    account
    |> ensureUsernameIsAvailable existingAccounts
    |> Result.bind (ensureEmailAddressIsAvailable existingAccounts)
```