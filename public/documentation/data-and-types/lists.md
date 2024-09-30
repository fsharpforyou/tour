# Lists

In F#, lists are an immutable series of elements of the same type implemented as a singly linked list. You can define a list by surrounding semicolon-separated values with square brackets.

```fsharp
let numbers = [ 1; 2; 3 ]
printfn "%A" numbers
```

There are two primary ways to add values to a list: You can prepend elements using the `::` operator, and concatenate two lists using the `@` operator.

```fsharp
let numbers = [ 1; 2; 3 ]
let numbers2 = 0 :: numbers
let numbers3 = numbers2 @ [4; 5; 6]
printfn "%A" numbers3
```

Each list has a `head` and a `tail`. The `head` is the first element of the list, and the `tail` is every subsequence element.

```fsharp
let numbers = [1; 2; 3]
let head = List.head numbers
let tail = List.tail numbers
printfn "Head = %A, Tail = %A" head tail
```

There are two patterns that allow us to match against and deconstruct list values. The _list_ pattern and the _cons_ pattern.

The _list_ pattern allows you to supply a pattern for each value in a list.

```fsharp
let numbers = [1]
match numbers with
| [] -> printfn "The list is empty"
| [a] -> printfn $"The list has one element: {a}"
| _ -> printfn "???"
```

The _cons_ pattern allows you to deconstruct a list into N elements and the tail.

```fsharp
let numbers = [1; 2; 3]
match numbers with
| [] -> printfn "The list is empty"
| head :: tail -> printfn $"Head: {head}, Tail: {tail}"
```

The `head :: tail` pattern will deconstruct the list `[1; 2; 3]` into `head = 1` and `tail = [2; 3]`. This can also be done for N number of elements: `first :: second :: tail`. The `head :: tail` pattern will match against any list with a single element. While the `first :: second :: tail` pattern will match against any list with at least two elements, and so on.

The _cons_ pattern is often used in recursive functions. Using the _cons_ pattern we can process the first element in a list, transforming it, then passing the tail of the list back into the recursive function to continue.

```fsharp
let rec iter (f: 'a -> unit) (xs: 'a list) =
    match xs with
    | [] -> ()
    | x :: xs ->
        f x
        iter f xs

iter (printfn "%d") [1; 2; 3]
```

The `List` module contains common functions for operating with lists. These functions include but are not limited to:
* `map` which applies a transformation function to every element in a list.
* `filter` which removes elements from a list.
* `iter` which applies an `'a -> unit` function to each element in a list and returns `unit`.

```fsharp
let isEven x = x % 2 = 0
let square x = x * x

[0; 1; 2; 3; 4; 5;]
|> List.filter isEven // only even numbers
|> List.map square // square every number
|> List.iter (printfn "%d") // print every number
```