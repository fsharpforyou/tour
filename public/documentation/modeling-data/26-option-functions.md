# Working with options

## What you will learn

Option helpers transform present values while preserving absence.

Pattern matching remains the foundation. Once that shape is familiar, `Option.map` removes repetition:

```fsharp
let length = subtitle |> Option.map (fun text -> text.Length)
```

For `Some text`, the function runs and the result becomes `Some length`. For `None`, the result stays `None`. Its useful shape is:

```text
('a -> 'b) -> 'a option -> 'b option
```

The helper is equivalent to this match:

```fsharp
let mapOption transform optionalValue =
    match optionalValue with
    | Some value -> Some (transform value)
    | None -> None
```

Writing the match once removes mystery: `Option.map` packages a recurring two-branch transformation.

`Option.defaultValue` unwraps with a fallback:

```fsharp
let display = subtitle |> Option.defaultValue "No subtitle"
```

Use a default only when it tells the truth. `"No subtitle"` is a useful display value; a fabricated customer would hide the fact that lookup failed.

`Option.bind` is for a function that already returns an option. It avoids `Some (Some value)`:

```fsharp
let nonBlank text =
    if text = "" then None else Some text

let cleaned = subtitle |> Option.bind nonBlank
```

Choose a match when branches tell a domain story; choose helpers for a small standard transformation.

## Trace map and bind

For `Some "Earthsea" |> Option.map String.length`, map runs the function and returns `Some 8`. For `None`, it returns `None` without calling the function. Map never removes the optional layer.

If `nonBlank` returns `string option`, mapping it over another `string option` produces `string option option`: two independent layers of absence. `bind` flattens them into one. If the right helper is unclear, write the pattern match first; the branch types will show whether the next step returns a plain value or another option.

```fsharp
let bindOption next optionalValue =
    match optionalValue with
    | Some value -> next value
    | None -> None
```

`next` already returns the next option, so bind does not wrap it in another `Some`.

## Follow the types, not the helper names

Suppose `subtitle` is `string option` and `nonBlank` is
`string -> string option`:

```text
Option.map  nonBlank subtitle : string option option
Option.bind nonBlank subtitle : string option
```

With `map`, the outer option describes whether a subtitle existed and the inner
one describes whether its cleaned string was nonblank. With `bind`, either reason
for absence becomes the same `None`. Use `bind` when that flattening matches the
meaning of the workflow.

`Option.defaultValue` ends optional processing by choosing an ordinary value.
Once you default to a string, later code cannot distinguish a missing subtitle
from a real subtitle containing the same text. Put defaults near display or
other boundaries where losing that distinction is intentional.

## Read a common type error

If a function needs `string` but receives `string option`, F# is not asking for
a cast. It is pointing out an unhandled possibility. Match the option, map a
function over it, or choose an honest default. Each choice states what `None`
means.

## Try it

- Map a title to uppercase.
- Compare `map nonBlank` with `bind nonBlank`.
- Supply a different default.
- Pass `Some "A Novel"` to a function requiring `string`. Read the mismatch,
  then repair it by matching or choosing an honest default.

## Summary

`map` transforms a possible value, `bind` chains a possibly absent result, and `defaultValue` chooses a fallback.
