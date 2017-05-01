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
      let mockTopic exists get publish subscribe =
        { new JsInterop.Topic with
            member __.exists () = exists
            member __.get (?opts) = get opts
            member __.publish (msgs,?opts) = publish (unbox <| box <| msgs) opts
            member __.subscribe (?subName,?opts) = subscribe subName opts }
      describe "exists" <| fun _ ->
        let makeMock exists apiResp =
          mockTopic (Promise.lift (exists, apiResp)) undef undef undef
        itPromises "returns expected result when wrapped topic does not exist" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedResult = false
          let mockTopic = makeMock expectedResult expectedApiResp
          let testTopic = Topic mockTopic
          Topic.exists testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedResult)
            Assert.strictEqual(apiResp, expectedApiResp)
          )
        itPromises "returns expected result when wrapped topic exists" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedResult = true
          let mockTopic = makeMock expectedResult expectedApiResp
          let testTopic = Topic mockTopic
          Topic.exists testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedResult)
            Assert.strictEqual(apiResp, expectedApiResp)
          )
      describe "get" <| fun _ ->
        let makeMock getFun =
          mockTopic undef getFun undef undef
        itPromises "returns a wrapped topic from internal get call with no options" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedInnerTopic = mockTopic undef undef undef undef
          let mockGetFun opts =
            if opts = None then
              Promise.lift (expectedInnerTopic, expectedApiResp)
            else Unchecked.defaultof<_>
          let mockTopic = makeMock mockGetFun
          let testTopic = Topic mockTopic
          let expectedTopic = Topic expectedInnerTopic
          Topic.get testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedTopic)
            Assert.strictEqual(apiResp, expectedApiResp)
          )
      describe "ensureExists" <| fun _ ->
        let makeMock getFun =
          mockTopic undef getFun undef undef
        itPromises "returns a wrapped topic from internal get call with auto-create" <| fun () ->
          let expectedApiResp = { new ApiResponse }
          let expectedInnerTopic = mockTopic undef undef undef undef
          let mockGetFun opts =
            match opts with
            | Some { autoCreate = ac } when ac = true ->
              Promise.lift (expectedInnerTopic, expectedApiResp)
            | _ -> undef
          let mockTopic = makeMock mockGetFun
          let testTopic = Topic mockTopic
          let expectedTopic = Topic expectedInnerTopic
          Topic.ensureExists testTopic
          |> Promise.map (fun (res, apiResp) ->
            Assert.strictEqual(res, expectedTopic)
            Assert.strictEqual(apiResp, expectedApiResp)
          )

tests ()
