# Records

Records represent an immutable series of named values.

```fsharp
type Person = { FirstName: string; LastName: string }
```

You can create an instance of this record type by supplying a value for each named property.

```fsharp
let johnDoe = { FirstName = "John"; LastName = "Doe" }
```

You can access individual properties using the _dot notation_.

```fsharp
let johnDoe = { FirstName = "John"; LastName = "Doe" }
let fullName = johnDoe.FirstName
```

As record values are immutable, their properties can't be changed after creation. Instead, you can copy the contents of a record and update a subset of properties using the _copy and update expression_.

```fsharp
let janeDoe = { johnDoe with FirstName = "Jane" }
```

Here, all the properties of `johnDoe` are copied and the `FirstName` is set to `"Jane"` instead.

As F# is evaluated from top to bottom, the instance of a record value will be inferred by finding the closest record type with matching properties.

```fsharp
type Person = { FirstName: string; LastName: string }
type Customer = { FirstName: string; LastName: string }

let displayPerson (person: Person) = printfn "Person = %A" person
let displayCustomer (customer: Customer) = printfn "Customer = %A" customer

let johnDoe = { FirstName = "John"; LastName = "Doe" }
displayCustomer johnDoe

let johnDoe2: Person = { FirstName = "John"; LastName = "Doe" }
displayPerson johnDoe2

let johnDoe3 = { Person.FirstName = "John"; Person.LastName = "Doe" }
displayPerson johnDoe3
```

You can pattern match a record value using the _record pattern_. This pattern allows you to specify a pattern for one or more properties of a record.

```fsharp
type Person = { FirstName: string; LastName: string }

let identify person =
    match person with
    | { FirstName = "John"; LastName = "Doe" }
    | { FirstName = "Jane"; LastName = "Doe" } -> "Could not identify this person."
    | { FirstName = firstName; LastName = lastName } -> $"Identified as: {firstName} {lastName}"
```

```fsharp
type Person = { FirstName: string; LastName: string; Age: int }
type Team = { Name: string; Members: Person list }

let team =  {
  Name = "Developers"; 
  Members = [ 
    { FirstName = "John"; LastName = "Doe"; Age = 30 }
    { FirstName = "Jane"; LastName = "Smith"; Age = 28 }
  ]
}

let describeTeam team =
    let memberDescriptions =
        team.Members
        |> List.map (fun person -> $"{person.FirstName} {person.LastName} ({person.Age} years old)")
        |> String.concat ", "

    $"Team: {team.Name}, Members: {memberDescriptions}"

let teamDescription = describeTeam team
printfn "%s" teamDescription
```