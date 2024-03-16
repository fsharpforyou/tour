# Interfaces

An interface defines a contract or set of obligations that implementing types must fullfill.

```fsharp
type IDrawable =
    abstract member Draw : float * float -> unit 
```

The above drawable interface provides a `Draw` member that implementing types must provide an implementation for.
These interfaces serve as contracts that must be fulfilled by the client.

```fsharp
type Square(size: int) =
    interface IDrawable with
        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square @ X: {x}, Y: {y}"

let square: IDrawable = Square(10)
square.Draw(10, 20)
```

In the above example, the square object implements the interface `IDrawable` and provides an implementation for the `Draw` member that matches the signature defined in the interface. Also notice how the Square type is annotated as an `IDrawable`. In F#, to call interface members like `Draw`, a type must be convered from the concrete implementation, in this case a `Square`, into an instance of the interface, an `IDrawable`.

If you don't have an instance of `IDrawable`, you can convert an instance of `Square` into one by upcasting it using the cast (`:>`) operator.

```fsharp
type Square(size: int) =
    interface IDrawable with
        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square @ X: {x}, Y: {y}"

let square = Square(10)

let drawable = square :> IDrawable
drawable.Draw(10, 20)
```

Interfaces can also implement other interfaces! This would require the implementing type to supply implementations for abstract members present in both interfaces like so:

```fsharp
type IShape =
    abstract member Name: string

type IDrawable =
    inherit IShape
    abstract member Draw : float * float -> unit 
    
type Square() =
    interface IDrawable with
        member this.Name = "Square"
        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square @ X: {x}, Y: {y}"
```