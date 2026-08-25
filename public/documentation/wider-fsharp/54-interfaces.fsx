type IDisplayable =
    abstract member Display: unit -> string

type Shelf(label: string, capacity: int) =
    member _.Label = label
    member _.Capacity = capacity

    interface IDisplayable with
        member _.Display() =
            $"%s{label}: space for %d{capacity} books"

type CustomerLabel(name: string) =
    interface IDisplayable with
        member _.Display() = "Customer: " + name

let display (item: IDisplayable) = item.Display()

let shelf = Shelf("Science fiction", 40)
let customer = CustomerLabel("Ada")

printfn "%s" (display shelf)
printfn "%s" (display customer)

// Add an OrderLabel class that implements IDisplayable.
