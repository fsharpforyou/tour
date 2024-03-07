# Patterns and Match Expressions

Pattern matching allows you to match a value against patterns, which act as rules for their transformation. These patterns can bind values to names and deconstruct or decompose values into their constituent parts.

To demonstrate pattern matching, let's get started with a pattern you're already familiar with: the _variable_ pattern. This pattern allows you to bind a value to a name like so: `let five = 5`. That's right, you've been using the variable pattern the whole time! Just like how everything on the right-hand side of the equals sign in a binding is an expression, the left-hand side is always a pattern.

```fsharp
let <pattern> = <expression>
```

This is limited as the pattern needs to be exhaustive. Exhaustivity means that every possible value is accounted for by the given pattern. Because the variable pattern will bind any value to a name, it will always match against a value and is therefore exhaustive. However, not all patterns are exhaustive and you may want to attempt to match a value against a set of patterns. You can use match expressions to do this. You can declare a match expression like so:

```fsharp
let number = 10
let result =
    match number with
    | 10 -> "The number is Ten"
    | number -> $"The number is not ten, but instead: {number}"
```

Here you can see two patterns in action: the _constant_ and _variable_ patterns. The _constant_ pattern will match a value against a constant value like `10` or `"Hello, World!"`. This match expression is exhaustive as the last branch utilizes the _variable_ pattern which will always match against the value.

When you don't care about the value but still require exhaustivity, you can use the _wildcard_ pattern to discard it. 

```fsharp
let number = 10
let result =
    match number with
    | 10 -> "The number is Ten"
    | _ -> $"The value was discarded"
```

A branch in a match expression can also include a conditional expression. This is often used in conjunction with the _variable_ pattern to check if the bound value passes a conditional check.

```fsharp
let number = 10
let result =
    match number with
    | number when number % 2 = 0 -> $"{number} is even"
    | number -> $"{number} is odd"
```

Two patterns can lead to the same expression being evaluated using the _OR_ pattern.

```fsharp
let number = 10
let result =
    match number with
    | pattern1
    | pattern2 -> ""
    | _ -> ""
```

You can require that a value matches against two or more patterns using the _AND_ pattern.

```fsharp
let number = 10
let result =
    match number with
    | pattern1 & pattern2 -> ""
    | _ -> ""
```