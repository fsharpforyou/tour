# Abstract Classes and Inheritance

Objects can also inherit functionality from other objects. Inheriting creates a hierarchical relationship between two objects (parent and child), where the child has all the behavior of the parent, but implements abstract members that are present, but not implemented, in the parent object.

Abstract classes, unlike interfaces, can have: behavior, data, and abstract members. To define an abstract class you have to annotate the type with `[<AbstractClass>]`.

```fsharp
[<AbstractClass>]
type Drawable() =
    member _.Description = "A drawable shape."
    abstract member Draw : float * float -> unit 
```

Then, you can inherit the behavior and/or data from this type while also implementing the abstract `Draw` member.

```fsharp
[<AbstractClass>]
type Drawable() =
    member _.Description = "A drawable shape."
    abstract member Draw : float * float -> unit 

type Square(size: int) =
    inherit Drawable()
//                  ^^
// make sure to pass any required constructor parameters
    
    override this.Draw(x: float, y: float) =
        printfn $"Drawing a square @ X: {x}, Y: {y}"

let square = Square(10)
square.Description // "A drawable shape."
square.Draw(10, 20) // "Drawing a square @ X: 10, Y: 20" 
```

You can inherit behavior and data from non-abstract classes too. The difference being, abstract classes can provide abstract members that inheritors must provide an implementation for, while normal classes can not.

```fsharp
type Rockstar() =
    member _.PlayMusic() = ...
    abstract member Sing: unit -> unit

type FreddieMercury() =
    inherit Rockstar()
    override this.Sing() = ...

let rockstar = Rockstar()
rockstar.PlayMusic()

let freddie = FreddieMercury()
freddie.PlayMusic() // inherits behavior of the parent.
freddie.Sing() // overrides abstract behavior in the parent.
```