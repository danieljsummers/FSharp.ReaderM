[<AutoOpen>]
module FSharp.ReaderM

/// Reader monad
type ReaderM<'d, 'out> =
  'd -> 'out

/// Operations on reader monads
module Reader =
  
  (* --- Basic Operations --- *)

  /// Run a reader monad using the expected dependency
  let run dep (rm : ReaderM<_,_>) =
    rm dep
  
  /// Return a contstant from a reader monad
  let constant (c : 'c) : ReaderM<_,'c> =
    fun _ -> c
  
  (* Lifting of Functions and State *)

  /// Create (lift) a ReaderM from a one-parameter function
  let lift1 (f : 'd -> 'a -> 'out) : 'a -> ReaderM<'d, 'out> =
    fun a dep -> f dep a

  /// Create (lift) a ReaderM from a two-parameter function 
  let lift2 (f : 'd -> 'a -> 'b -> 'out) : 'a -> 'b -> ReaderM<'d, 'out> =
    fun a b dep -> f dep a b

  /// Create (lift) a ReaderM from a three-parameter function
  let lift3 (f : 'd -> 'a -> 'b -> 'c -> 'out) : 'a -> 'b -> 'c -> ReaderM<'d, 'out> =
    fun a b c dep -> f dep a b c

  /// Lift dependencies for use in satisfying the required input parameter for a Reader monad
  let liftDep (proj : 'd2 -> 'd1) (rm : ReaderM<'d1, 'output>) : ReaderM<'d2, 'output> =
    proj >> rm

  (* --- Functor --- *)
  
  /// Create a function that maps based on the output of function two and the input of function one
  let fmap (f : 'a -> 'b) (g : 'c -> 'a) : ('c -> 'b) =
    g >> f
  
  /// Map the output of a Reader monad
  let map (f : 'a -> 'b) (rm : ReaderM<'d, 'a>) : ReaderM<'d,'b> =
    rm >> f
  
  /// Map the output of a Reader monad
  let (<?>) = map
  
  (* --- Applicative-Functor --- *)

  /// Apply a function, return the result as a ReaderM
  let apply (f : ReaderM<'d, 'a->'b>) (rm : ReaderM<'d, 'a>) : ReaderM<'d, 'b> =
    fun dep ->
      let f' = run dep f
      let a  = run dep rm
      f' a
  
  /// Apply a function, remaining lifted as a ReaderM
  let (<*>) = apply
  
  (* --- Monad --- *)
  
  /// Bind a parameter to a reader monad
  let bind (rm : ReaderM<'d, 'a>) (f : 'a -> ReaderM<'d,'b>) : ReaderM<'d, 'b> =
    fun dep ->
      f (rm dep) 
      |> run dep 
  
  /// Bind a parameter to a reader monad
  let (>>=) = bind

  /// Computation expression builder for the reader monad
  type ReaderMBuilder internal () =
    member __.Bind (m, f) =
      m >>= f
    member __.Return (v) =
      constant v
    member __.ReturnFrom (v) =
      v
    member __.Delay (f) =
      f ()

/// Reader monad computation expression
let reader = Reader.ReaderMBuilder()
