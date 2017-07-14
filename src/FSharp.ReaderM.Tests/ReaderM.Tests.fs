module FSharp.ReaderM.Tests

open Expecto
open Reader

(* --- Interfaces (the shape of our "dependencies") for our test classes to implement --- *)

type IAddOneToOne   = abstract Add: int -> int
type IAddOneToTwo   = abstract Add: int -> int -> int
type IAddOneToThree = abstract Add: int -> int -> int -> int

(* --- Implementations of the IAddOneTo* interfaces --- *)

let addToOne =
  { new IAddOneToOne with
      member __.Add nbr = nbr + 1
    }

let addToTwo =
  { new IAddOneToTwo with
      member __.Add nbr1 nbr2 = nbr1 + nbr2 + 1
    }

let addToThree =
  { new IAddOneToThree with
      member __.Add nbr1 nbr2 nbr3 = nbr1 + nbr2 + nbr3 + 1
    }

(* --- Functions to extract our required item from the depenencies --- *)
let getAddOne   (deps : IAddOneToOne)   = deps.Add
let getAddTwo   (deps : IAddOneToTwo)   = deps.Add
let getAddThree (deps : IAddOneToThree) = deps.Add


module Tests =

  [<Tests>]
  let testLiftAndRun =
    testList "lift and run" [
      test "succeeds with one-parameter function" {
        let value = lift1 getAddOne 5 |> run addToOne
        Expect.equal value 6 "should have been 5 + 1 (6)"
        }
      test "succeeds with two-parameter function" {
        let value = lift2 getAddTwo 6 7 |> run addToTwo
        Expect.equal value 14 "should have been 6 + 7 + 1 (14)"
        }
      test "succeeds with three-parameter function" {
        let value = lift3 getAddThree 10 20 30 |> run addToThree
        Expect.equal value 61 "should have been 10 + 20 + 30 + 1 (61)"
        }
      ]
  
  [<Tests>]
  let testConstantAndRun =
    testList "constant and run" [
      test "succeeds" {
        let value = constant 4 |> run addToOne
        Expect.equal value 4 "should have returned the given constant value"
        }
      ]
  
  [<Tests>]
  let testLiftDepAndRun = 
    testList "liftDep and run" [
      test "succeeds" {
        let value = liftDep getAddOne (fun x -> x 4) |> run addToOne
        Expect.equal value 5 "should have been 4 + 1 (5)"
        }
      ]

  [<Tests>]
  let testFmap =
    testList "fmap" [
      test "succeeds" {
        let mapped = fmap (string) (int64)
        Expect.equal (mapped 2uy) "2" "should have converted byte to long to string"
      }
    ]
  
  [<Tests>]
  let testMap =
    testList "map and run" [
      test "succeeds" {
        let mapped = string <?> lift1 getAddOne 7
        Expect.equal (run addToOne mapped) "8" "should have mapped the output to a string"
        }
      ]
  
  [<Tests>]
  let testApply =
    testList "apply and run" [
      test "succeeds" {
        let toString x = fun _ -> string x
        let applied = lift1 toString <*> lift1 getAddOne 14
        Expect.equal (run addToOne applied) "15" "should have applied the 'toString' reader"
        }
      ]
  
  [<Tests>]
  let testBind =
    testList "bind and run" [
      test "succeeds" {
        let bound = lift1 getAddOne 44 >>= lift1 getAddOne
        Expect.equal (run addToOne bound) 46 "should have added 1 twice"
        }
      ]
  
  [<Tests>]
  let testBuilder =
    testList "builder" [
      test "succeeds for return!" {
        let rm =
          reader {
            return! lift1 getAddOne 5
            }
        Expect.equal (run addToOne rm) 6 "should have added one"
        }
      test "succeeds for return" {
        let rm =
          reader {
            return 2
            }
        Expect.equal (run addToOne rm) 2 "should not have added one; returning a constant"
        }
      test "succeeds for bind" {
        let rm =
          reader {
            let! x = lift1 getAddOne 98
            return x
            }
        Expect.equal (run addToOne rm) 99 "should have added one"
        }
      ]


/// Run 'dem tests!
[<EntryPoint>]
let main argv =
  let cfg = { defaultConfig with verbosity = Logging.Debug }
  runTestsInAssembly cfg argv