[<AbstractClass>]
type Drawable() =
    member _.Description = "A drawable shape."
    abstract member Draw: float * float -> unit

type Square(size: int) =
    inherit Drawable()

    override this.Draw(x: float, y: float) =
        printfn $"Drawing a square @ X: {x}, Y: {y}"

let square = Square(10)
square.Draw(0, 0)
