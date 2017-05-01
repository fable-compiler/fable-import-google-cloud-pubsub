module Tests

open Fable.Core
open Fable.Import
open Util
open Fable.Import.Google.Cloud
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
    describe "Topic" <| fun _ ->
      describe "exists" <| fun _ ->
        itPromises "returns expected result when wrapped topic does not exist" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedResult = false
          let mockTopic =
            { new JsInterop.Topic with
                member __.exists () = Promise.lift (expectedResult, expectedApiResp)
                member __.get (?opts) = Unchecked.defaultof<_>
                member __.publish (msgs,?opts) = Unchecked.defaultof<_>
                member __.subscribe (?subName,?opts) = Unchecked.defaultof<_> }
          let testTopic = Topic mockTopic
          Topic.exists testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedResult)
            Assert.strictEqual(apiResp, expectedApiResp)
          )
        itPromises "returns expected result when wrapped topic exists" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedResult = true
          let mockTopic =
            { new JsInterop.Topic with
                member __.exists () = Promise.lift (expectedResult, expectedApiResp)
                member __.get (?opts) = Unchecked.defaultof<_>
                member __.publish (msgs,?opts) = Unchecked.defaultof<_>
                member __.subscribe (?subName,?opts) = Unchecked.defaultof<_> }
          let testTopic = Topic mockTopic
          Topic.exists testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedResult)
            Assert.strictEqual(apiResp, expectedApiResp)
          )
      describe "get" <| fun _ ->
        itPromises "returns a wrapped topic from internal get call with no options" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedInnerTopic =
            { new JsInterop.Topic with
                member __.exists () = Unchecked.defaultof<_>
                member __.get (?opts) = Unchecked.defaultof<_>
                member __.publish (msgs,?opts) = Unchecked.defaultof<_>
                member __.subscribe (?subName,?opts) = Unchecked.defaultof<_> }
          let mockTopic =
            { new JsInterop.Topic with
                member __.exists () = Unchecked.defaultof<_>
                member __.get (?opts) =
                  if opts = None then
                    Promise.lift (expectedInnerTopic, expectedApiResp)
                  else Unchecked.defaultof<_>
                member __.publish (msgs,?opts) = Unchecked.defaultof<_>
                member __.subscribe (?subName,?opts) = Unchecked.defaultof<_> }
          let testTopic = Topic mockTopic
          let expectedTopic = Topic expectedInnerTopic
          Topic.get testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedTopic)
            Assert.strictEqual(apiResp, expectedApiResp)
          )
      describe "ensureExists" <| fun _ ->
        itPromises "returns a wrapped topic from internal get call with auto-create" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedInnerTopic =
            { new JsInterop.Topic with
                member __.exists () = Unchecked.defaultof<_>
                member __.get (?opts) = Unchecked.defaultof<_>
                member __.publish (msgs,?opts) = Unchecked.defaultof<_>
                member __.subscribe (?subName,?opts) = Unchecked.defaultof<_> }
          let mockTopic =
            { new JsInterop.Topic with
                member __.exists () = Unchecked.defaultof<_>
                member __.get (?opts) =
                  if opts = Some TopicGetOptions.withAutoCreate then
                    Promise.lift (expectedInnerTopic, expectedApiResp)
                  else Unchecked.defaultof<_>
                member __.publish (msgs,?opts) = Unchecked.defaultof<_>
                member __.subscribe (?subName,?opts) = Unchecked.defaultof<_> }
          let testTopic = Topic mockTopic
          let expectedTopic = Topic expectedInnerTopic
          Topic.ensureExists testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedTopic)
            Assert.strictEqual(apiResp, expectedApiResp)
          )

tests ()
