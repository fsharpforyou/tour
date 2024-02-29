# Sequences

In F#, sequences are a lazily-evaluated, potentially infinite, immutable series of elements of the same type. You can define a sequence using the `seq` computation expression.

```fsharp
seq { 1; 2; 3 }
```

The `Seq` module, like the `List` module, includes helper functions for manipulating sequences in certain ways.
Many of these functions are also present in the `List` module, such as `map`, `filter`, `head`, etc...

```fsharp
let double x = x * 2

let sequence = seq { 1; 2; 3 }
let doubledSequence = Seq.map double sequence
```

Unlike with Lists, which are eagerly evaluated, the elements of a sequence are only evaluated/produced when necessary. We can see this by creating an infinite sequence from `1` to `infinity` and only evaluating the first 10 elements.

```fsharp
Seq.initInfinite (fun x -> x * 2) // produce numbers 1 to infinity and multiply each by two
|> Seq.take 10 // take the first 10 numbers
|> Seq.iter (printfn "%d") // print each number to the console.
```