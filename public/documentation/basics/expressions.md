# Expressions

The primary piece of F# syntax is an expression. An expression is simply, a block of code that when evaluated, produces a value. 

_Let bindings_ allow you to bind the result of an expression to a name. 

```fsharp
let number = 5
```

Everything on the right-hand side of the equal sign is an expression. The last expression within an expression block is what produces a result for the expression as a whole.

```fsharp
let ten =
    let five = 5
    five + five
```

As you may notice, the `ten` binding doesn't include a type annotation. This is because the F# compiler can infer the types of values from their usage. In this case, `ten` is inferred as an `int` although it can easily be several other numerical types. If the compiler doesn't have enough information to infer your desired type, you can add a type annotation to override it.

```fsharp
let ten: float = 10
```

By default, let bindings are immutable. Immutability is highly preferred as it ensures that values won't be changed unexpectedly and results in code that's easier to reason about.

In F#, code is evaluated from top to bottom which has some interesting side effects. You can only reference bindings, functions, or types that are defined above the current definition.

```fsharp
ten + ten
let ten = 10
// `ten` is defined below its usage which results in a compiler error.
```

The advantage of top-down evaluation is that the flow of your application's code is easier to reason about. Code is read and evaluated from top to bottom sequentially, from a higher to a lower level, from core components to specific implementation details. We can clearly understand and reason about our dependent modules, types, functions and their dependencies.

This top-down evaluation also applies to the ordering of files within a project. You can only use functions, types, and modules defined in other files if the file is ordered above the current definition.

```
1. Logic.fs
2. Program.fs
```

Here, any code in `Program.fs` can access modules, types, functions, and bindings in `Logic.fs`, but not the other way around. This is because `Logic.fs` is ordered above `Program.fs`.

F# relies on the level of indentation to determine the beginning and end of an expression. You may be familiar with this if you've programmed in languages with syntatic indentation before. When writing an expression block, the level of indentation must be consistent for each expression within that block.

```fsharp
let ten =
    let five = 5 // <---
    five + five  // <---
                 // this block has consistent indentation levels.

let twenty = ten + ten
```

Here, the compiler can distinguish between the beginning and the end of the expression. Both of these lines are indented consistently with 4 spaces and are terminated by the subsequent non-indented expression.