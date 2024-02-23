let add x y = x + y

let addFive = add 5 // partially applied
let fifteen = addFive 10
printfn "%d" fifteen

let criticalOperation (logger: string -> unit) (value: string) = logger $"{value}!!!"
let criticalOperationWithConsoleLogger = criticalOperation (printfn "%s")
criticalOperationWithConsoleLogger "Hello, World!"
