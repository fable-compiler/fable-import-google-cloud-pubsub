module Tests

open Fable.Core
open Fable.Import
open Util
open Fable.Import.Google.Cloud.PubSub

let tests () =
  describe "Google PubSub" <| fun _ ->
    it "exists" <| fun () ->
      Assert.ok(JsInterop.pubsub)
    describe "TopicGetOptions" <| fun _ ->
      it "withAutoCreate has expected value" <| fun () ->
        Assert.strictEqual(TopicGetOptions.withAutoCreate.autoCreate, Some true)
    describe "PullOptions" <| fun _ ->
      it "onlyOne pulls only one item" <| fun () ->
        Assert.strictEqual(PullOptions.onlyOne.maxResults, Some 1)
      it "nonBlocking returns immediately" <| fun () ->
        Assert.strictEqual(PullOptions.nonBlocking.returnImmediately, Some true)

tests ()
