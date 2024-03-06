let number = 20

let fizzBuzz =
    if number % 15 = 0 then "FizzBuzz"
    elif number % 5 = 0 then "Buzz"
    elif number % 3 = 0 then "Fizz"
    else string number

printfn "%s" fizzBuzz
