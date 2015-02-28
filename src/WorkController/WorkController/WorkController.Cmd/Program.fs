open FSharp.Configuration
open System
open System.Threading

let cprintf c fmt = 

    Printf.kprintf 
        (fun s -> 
            let old = System.Console.ForegroundColor 
            try 
              System.Console.ForegroundColor <- c;
              System.Console.Write s
            finally
              System.Console.ForegroundColor <- old) 
        fmt
        
let cprintfn c fmt = 
    cprintf c fmt
    printfn ""

let getWorkTime() = 
     AppSettings<"app.config">.WorkTime
   

let getEndWork (startHour :DateTime) workTime = 
    startHour.AddHours(if workTime > 0.0 then workTime else getWorkTime())

let printRemainingTime (r :TimeSpan) (e :DateTime) = 
    printfn "It is %dh and %dm to %s ( end of the day )"   r.Hours r.Minutes (e.ToShortTimeString())

let riseEndDay() =
    cprintfn ConsoleColor.Red "End of work for today. Go HOME !!!"

let work  (s:DateTime) (e:DateTime) =
    let rec loop (c :DateTime) ll =
        let r = e - c
        match r with 
            | (var1)  when var1.TotalMinutes > 0.0  -> 
                let mutable lastLogedTime = ll
                if (var1.Minutes % 2 = 0 ) && (var1.Minutes <> lastLogedTime )  then
                    printRemainingTime var1 e
                    lastLogedTime  <-  var1.Minutes
                Thread.Sleep (1000)
                loop (DateTime.Now) lastLogedTime
            | _ -> riseEndDay()

    //printRemainingTime (e-s) e
    loop s  s.Minute       

 

let getStartAndEndTime(workTime) =
    let s = DateTime.Now
    let e = getEndWork s workTime
    (s,e)



[<EntryPoint>]
[<STAThread>]
let main argv = 

    let args = argv |> List.ofSeq
    let workTime = match (args) with
      | first :: [] -> 
        float(first)
      | _ -> 
        printfn "Usage : <app.exe> <worktime:float>"
        0.0
    
    let (s,e) = getStartAndEndTime(workTime)
    work s e
    Console.ReadKey() |> ignore
    
    0 // return an integer exit code