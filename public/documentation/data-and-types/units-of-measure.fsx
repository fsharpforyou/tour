[<Measure>]
type mile

[<Measure>]
type kilometer

let miles = 10<mile>
let kilometers = 10<kilometer>

let calculateDistanceBetween (startMile: int<mile>) (endMile: int<mile>) = endMile - startMile


let startMile = 10<mile>
let endMile = 20<mile>
let distance = calculateDistanceBetween startMile endMile
printfn "Distance = %d" distance
