type IShape =
    abstract member Name: string

type IDrawable =
    inherit IShape
    abstract member Draw: float * float -> unit

type Square(size: int) =
    interface IDrawable with
        member this.Name = "Square"

        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square with a size of {size} @ X: {x}, Y: {y}"


let drawableShape: IDrawable = Square(10)
printfn $"Shape Name: {drawableShape.Name}"
drawableShape.Draw(10, 20)
