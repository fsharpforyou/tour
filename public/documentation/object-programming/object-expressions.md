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

let square =
    { new IShape with
        member this.Kind = "square"
        
      new IDrawable with
        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square @ X: {x}, Y: {y}" }

square.Kind // "square"
(square :> IDrawable).Draw(0.0, 0.0) // "Drawing a square @ X: 0.0, Y: 0.0"
```

Notice that the type of `square` is an `IShape` as that's what its created as when using `new IShape with`. It just so happens that it also implements the `IDrawable` interface and casting is required to convert it to an `IDrawable`