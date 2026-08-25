type Dispatch = { OrderNumber: int; ShipOnDay: int }

let dispatches = [
    { OrderNumber = 101; ShipOnDay = 105 }
    { OrderNumber = 102; ShipOnDay = 125 }
    { OrderNumber = 103; ShipOnDay = 110 }
]

let imminentLabels =
    dispatches
    |> Seq.filter (fun dispatch -> dispatch.ShipOnDay <= 110)
    |> Seq.map (fun dispatch -> $"Order %d{dispatch.OrderNumber} ships by day %d{dispatch.ShipOnDay}")

let displayedLabels = imminentLabels |> Seq.toList

let dispatchWindows =
    seq {
        for week in 1..3 do
            yield 100 + week * 7
    }
    |> Seq.toList

printfn "Imminent dispatches: %A" displayedLabels
printfn "Future dispatch windows: %A" dispatchWindows

// Try enumerating imminentLabels twice and explain when its work is repeated.
