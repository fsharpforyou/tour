type User = { Id: int; Name: string }

let tryFindUserById id users =
    List.tryFind (fun user -> user.Id = id) users

let users = [ { Id = 1; Name = "John Doe" }; { Id = 2; Name = "Jane Doe" } ]
let foundUser = tryFindUserById 1 users
printfn "Found user: %A" foundUser
