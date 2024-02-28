# Lists

In F#, lists are an immutable series of elements of the same type implemented as a singly linked list. You can define a list by surrounding semicolon-separated values with square brackets.

```fsharp
let numbers = [ 1; 2; 3 ]
```

There are two primary ways to add values to a list: You can prepend elements using the `::` operator, and concatenate two lists using the `@` operator.

```fsharp
let numbers2 = 0 :: numbers
let numbers3 = numbers2 @ [4; 5; 6]
```

Each list has a `head` and a `tail`. The `head` is the first element of the list, and the `tail` is every subsequence element.

```fsharp
let numbers = [1; 2; 3]
let head = List.head numbers // 1
let tail = List.tail numbers // [2; 3]
```

There are two patterns that allow us to match against and deconstruct list values. The _list_ pattern and the _cons_ pattern.

The _list_ pattern allows you to supply a pattern for each value in a list.

```fsharp
let numbers = [1; 2; 3]
match numbers with
| [] -> "The list is empty"
| [a] -> $"The list has one element: {a}"
| ... -> ...
```

The _cons_ pattern allows you to deconstruct a list into N elements and the tail.

```fsharp
let numbers = [1; 2; 3]
match numbers with
| [] -> "The list is empty"
| head :: tail -> $"Head: {head}, Tail: {tail}"
```

The `head :: tail` pattern will deconstruct the list `[1; 2; 3]` into `head = 1` and `tail = [2; 3]`. This can also be done for N number of elements: `first :: second :: tail`. The `head :: tail` pattern will match against any list with a single element.