module Util

open Fable.Core
open Fable.Import

let Assert = Node.``assert``

type Mocha =
    [<Global>]
    abstract describe: string * (unit -> unit) -> unit
    [<Global>]
    abstract it: string * (unit -> unit) -> unit
    [<Global>]
    abstract it: string * ((unit -> unit) -> unit) -> unit
    [<Global>]
    abstract it: string * (unit -> JS.Promise<'a>) -> unit

[<Import("*","assert")>]
let m : Mocha = jsNative

let inline describe name tests = m.describe(name, tests)
let inline it name (tests : unit -> unit) = m.it(name, tests)
let inline itWithCallback name (tests : (unit -> unit) -> unit) = m.it(name, tests)
let inline itPromises name (tests : unit -> JS.Promise<_>) = m.it(name, tests)

let undef<'a> = Unchecked.defaultof<'a>

module Promise =
  [<Emit("Promise.resolve($0)")>]
  let lift<'T> (a: 'T): JS.Promise<'T> = jsNative

  [<Emit("$1.then($0)")>]
  let map (a: 'T->'R) (pr: JS.Promise<'T>): JS.Promise<'R> = jsNative
