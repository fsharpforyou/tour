let negative x = x * - 1
let double x = x * 2

3 |> negative |> double |> printfn "double(negative(3)) = %d"

let add x y = x + y
let multiply x y = x * y
let operation = add 3 >> multiply 3

let result = operation 5
printfn "multiply 3 (add 3 5) = %d" result
