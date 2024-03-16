type Person(firstName: string, lastName: string) =
    member val FirstName = firstName with get
    member val LastName = lastName with get

    member this.Greet(?greeting: string, ?punctuation: string) =
        let greeting = Option.defaultValue "Hello" greeting
        let punctuation = Option.defaultValue "!" punctuation
        $"{greeting} {this.FirstName} {this.LastName}{punctuation}"

let person = Person("John", "Doe")
let greeting = person.Greet(greeting = "Greetings", punctuation = "!!!")
printfn "%s" greeting
