# Object Expressions

An object expression allows us to create an anonymous object from an existing base type, interface, or abstract class.

```fsharp
type IDrawable =
    abstract member Draw : float * float -> unit 

let square =
    { new IDrawable with
        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square @ X: {x}, Y: {y}" }

square.Draw(0.0, 0.0) // "Drawing a square @ X: 0.0, Y: 0.0"
```

An object expression can also implement more than one interface.

```fsharp
type IShape =
    abstract Kind: string

type IDrawable =
    abstract member Draw : float * float -> unit

type ISquare =
  inherit IShape
  inherit IDrawable

let square =
    { new ISquare with
        member this.Kind = "square"

        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square @ X: {x}, Y: {y}" }

square.Kind // "square"
square.Draw(0.0, 0.0) // "Drawing a square @ X: 0.0, Y: 0.0"
```

Notice that the type of `square` is an `ISquare` as that's what it's created as when using `new ISquare with`.
