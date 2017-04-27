module Tests

open Fable.Core
open Fable.Import
open Util
open Fable.Import.Google.Cloud.PubSub

let tests () =
  describe "Google PubSub" <| fun _ ->
    it "exists" <| fun () ->
      Assert.ok(pubsub)

tests ()
