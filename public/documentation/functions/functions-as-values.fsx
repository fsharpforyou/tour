let double x = x * 2
let apply (f: int -> int) = f 5

let ten = apply double
printfn "5 * 2 = %d" ten

let twentyFive = apply (fun value -> value * 5)
printfn "5 * 5 = %d" twentyFive
