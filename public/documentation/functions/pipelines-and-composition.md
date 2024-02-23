# Pipelines and Composition

The pipeline operator `|>` is a very simple operator that allows you to pipe a value into a function. We can see this by defining our own version of it.

```fsharp
let (|>) value f = f value
```

With the pipe operator, the function application of `f(g(x))` can be replaced with `x |> g |> f`. First, the value of `x` is applied to the function `g` and the result is applied to the function `f`.

As the pipeline operator applies a value to a single parameter function (all F# functions), often times it's used in conjunction with partial application.

```fsharp
let add x y = x + y

// produces int -> int function (not what we want)
5 |> add

// `add 3` is evaluated - producing an int -> int function
// 5 is piped into that function.
5 |> add 3
```

You can also combine two or more functions into a single function with the composition operator: `>>`. Let's define our own version of it to see how it works.

```fsharp
let (>>) function1 function2 =
    fun value ->
        function2(function1(value))
```

As you can see, a value is passed into the left-hand function and the result is passed into the right-hand function as an input. The function definition of `let func x = f(g(x))` can be replaced with `let func = g >> f`. 

As you may notice, this operator is also often used in conjunction with partial application. This operator will create an `a -> c` function from the usage `(a -> b) >> (b -> c)`

```fsharp
let add x y = x + y
let multiply x y = x * y
let operation = add 3 >> multiply 3
// equivalent to:
// let operation x = multiply 3 (add 3 x)
```

The expression `add 3 >> multiply 3` works because the function signature of each function is `int -> int`. This will result in a function with an implicit parameter that will first be passed into `add 3` and the resulting `int` value will be passed into `multiply 3`.