let number = 15

let fizzBuzz =
    match number with
    | number when number % 15 = 0 -> "FizzBuzz"
    | number when number % 5 = 0 -> "Buzz"
    | number when number % 3 = 0 -> "Fizz"
    | number -> string number

printfn "%s" fizzBuzz
