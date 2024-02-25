type Logger = string -> unit

let exclaim (logger: Logger) (value: string) = logger (sprintf "%s!!!" value)

let logger: Logger = printfn "%s"
exclaim logger "Hello"
