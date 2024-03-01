Seq.initInfinite (fun x -> x * 2) |> Seq.take 10 |> Seq.iter (printfn "%d")
