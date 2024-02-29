type Data<'a> = { Value: 'a }
let data: Data<string> = { Value = "Hello, World!" }
printfn "%s" data.Value
