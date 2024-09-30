# Pipelines and Composition

## The Pipeline Operator: `|>`

The pipeline operator `|>` allows you to easier pass a value to a function, making your code cleaner and more expressive. Let's start by defining our own version of the operator to see how it works.

```fsharp
let (|>) value f = f value
```

Using the pipeline operator, you can rewrite nested function calls like `f(g(x))` in a more readable way: `x |> g |> f`. First, the value of `x` is applied to the function `g` and the result is then applied to the function `f`.

```fsharp
let square x = x * x
let addOne x = x + 1

let result =
    4
    |> square
    |> addOne

printfn "Result = %d" result
```

In this case, the numebr `4` is squared to get `16`, and then added to `1` to get the value of `17`.

Because a single value is piped into a single argument function, which all F# functions are, the pipeline operator is often used in conjunction with partial application. Here's another example that demonstrates this:

```fsharp
let formatCurrency amount = sprintf "$%.2f" amount
let applyDiscount rate price = price - (price * rate)
let applyTax rate price = price + (price * rate)

let finalPrice =
    100.0
    |> applyDiscount 0.1
    |> applyTax 0.05

printfn "Final Price = %s" (formatCurrency finalPrice) 
```

Pay attention to the order of arguments in the `applyDiscount` and `applyTax` functions. In F#, it's common for the most important argument to be placed last. This design choice enhances the composability of functions. By positioning the `price` argument last, we can partially apply the `rate` argument, resulting in a function of type `float -> float` that can then be piped into or composed with other functions easily.

## Composing Functions With `>>`

You can compose two functions into a single function using the composition operator `>>`. Let's define our own version to understand how it works:

```fsharp
let (>>) f g =
    fun x -> g (f x)
```

As you can see, a value is passed into the left-hand function and the result is passed into the right-hand function as an input. The function definition of `let func x = f(g(x))` can be replaced with `let func = g >> f`. 

As you may notice, this operator is also often used in conjunction with partial application. This operator will create an `a -> c` function from the usage `(a -> b) >> (b -> c)`

```fsharp
let formatCurrency amount = sprintf "$%.2f" amount
let applyDiscount rate price = price - (price * rate)
let applyTax rate price = price + (price * rate)

let calculatePrice = applyDiscount 0.1 >> applyTax 0.05
let finalPrice = calculatePrice 100.0

printfn "Final Price = %s" (formatCurrency finalPrice)
```

The expression `applyDiscount 0.1 >> applyTax 0.05` works because the function signature of each function is `float -> float`. This will result in a function with an implicit parameter that will be applied to the function `applyDiscount 0.1` and the resulting `float` value will be applied to the function `applyTax 0.05`.