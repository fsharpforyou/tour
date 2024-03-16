type IDrawable =
    abstract member Draw: float * float -> unit

let square =
    { new IDrawable with
        member this.Draw(x: float, y: float) =
            printfn $"Drawing a square @ X: {x}, Y: {y}" }

square.Draw(0.0, 0.0)
