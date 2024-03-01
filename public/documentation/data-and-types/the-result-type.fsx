type Account =
    { Username: string
      EmailAddress: string }

type CreateAccountError =
    | UsernameNotAvailable
    | EmailAddressNotAvailable

let ensureUsernameIsAvailable existingAccounts account =
    if List.exists (fun x -> x.Username = account.Username) existingAccounts then
        Error UsernameNotAvailable
    else
        Ok account

let ensureEmailAddressIsAvailable existingAccounts account =
    if List.exists (fun x -> x.EmailAddress = account.EmailAddress) existingAccounts then
        Error EmailAddressNotAvailable
    else
        Ok account

let createAccount existingAccounts account =
    account
    |> ensureUsernameIsAvailable existingAccounts
    |> Result.bind (ensureEmailAddressIsAvailable existingAccounts)

let existingAcounts =
    [ { Username = "john"
        EmailAddress = "johndoe@site.com" }
      { Username = "jane"
        EmailAddress = "janedoe@site.com" } ]

let accountToCreate =
    { Username = "chris"
      EmailAddress = "chris@site.com" }

match createAccount existingAcounts accountToCreate with
| Ok account -> printfn "Created account: %A" account
| Error error -> printfn "Failed to create account: %A" error
