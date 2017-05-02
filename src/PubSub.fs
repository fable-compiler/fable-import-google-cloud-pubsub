module Fable.Import.Google.Cloud.PubSub

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import

open FSharp.Data.UnitSystems.SI.UnitNames

type [<Erase>] TopicName = TopicName of string
type [<Erase>] SubscriptionName = SubscriptionName of string
type [<Erase>] AckId = AckId of string
type [<Erase>] MessageId = MessageId of string

type SubscribeOptions =
  { ackDeadlineSeconds : int<second> option
    autoAck : bool option
    encoding : string option
    interval : int<millisecond> option
    maxInProgress : int option
    pushEndpoint : string option
    timeout : int<millisecond> option }

type SubscriptionOptions =
  { autoAck : bool option
    encoding : string option
    interval : int<millisecond> option
    maxInProgress : int option
    timeout : int<millisecond> option }

type TopicGetOptions =
  { autoCreate: bool }

module TopicGetOptions =
  let withAutoCreate =
    { autoCreate = true }

type PublishOptions =
  { raw: bool
    timeout: int<millisecond> }

type AckOptions =
  { timeout: int<millisecond> }

type PullOptions =
  { maxResults: int option
    returnImmediately: bool option }

module PullOptions =
  let onlyOne =
    { maxResults = Some 1
      returnImmediately = None }
  let nonBlocking =
    { maxResults = None
      returnImmediately = Some true }

type Message =
  { ackId: AckId
    id: MessageId
    data: string
    attributes: Map<string,string>
    timestamp: string }

type Options =
  { projectId: ProjectId option
    email: string option
    keyFilename: string option
    apiEndpoint: string option }

module JsInterop =
  type Subscription =
    abstract ack: [<ParamArray>] ackIds: AckId[] * ?options: AckOptions -> JS.Promise<unit>
    abstract on: eventType:string * listener:('a -> unit) -> unit
    abstract removeListener: eventType: string * listener: ('a -> unit) -> unit
    abstract pull: ?options:PullOptions -> JS.Promise<Message[] * ApiResponse>

  type Topic =
    abstract exists: unit -> JS.Promise<bool * ApiResponse>
    abstract get: ?options:TopicGetOptions -> JS.Promise<Topic * ApiResponse>
    abstract publish: [<ParamArray>] message: 'a[] * ?options: PublishOptions -> JS.Promise<MessageId[] * ApiResponse>
    abstract subscribe: ?subName: SubscriptionName * ?options: SubscribeOptions -> JS.Promise<Subscription * ApiResponse>
    abstract name: TopicName with get

  type PubSubConnection =
    abstract createTopic: topicName: TopicName -> JS.Promise<Topic * ApiResponse>
    abstract subscribe: topic: U2<Topic,TopicName> * ?subName: SubscriptionName * ?options: SubscribeOptions -> JS.Promise<Subscription * ApiResponse>
    abstract subscription: ?name: SubscriptionName * ?options: SubscriptionOptions -> Subscription
    abstract topic: name: TopicName -> Topic

  type PubSubProvider =
    [<Emit("$0($1)")>]
    abstract Init : ?options:Options -> PubSubConnection

  [<Import("default","@google-cloud/pubsub")>]
  let pubsub: PubSubProvider = jsNative

type [<Erase>] PubSub = PubSub of JsInterop.PubSubConnection
type [<Erase>] Topic = Topic of JsInterop.Topic
type [<Erase>] Subscription = Subscription of JsInterop.Subscription

module PubSub =
  let init () =
    JsInterop.pubsub.Init()
    |> PubSub
  let initWithOpts opts =
    JsInterop.pubsub.Init(opts)
    |> PubSub

  let topic topicName (PubSub pubsub) =
    pubsub.topic(topicName)
    |> Topic

module Topic =
  module private Promise =
      [<Emit("$1.then($0)")>]
      let map (a: 'T->'R) (pr: JS.Promise<'T>): JS.Promise<'R> = jsNative

  let exists (Topic topic) = topic.exists()
  let get (Topic topic) = topic.get() |> Promise.map (fun (t,x) -> Topic t, x)
  let getName (Topic topic) = topic.name
  let ensureExists (Topic topic) = topic.get(TopicGetOptions.withAutoCreate) |> Promise.map (fun (t,x) -> Topic t, x)
  let publishOne (Topic topic) (msg : 'a) = topic.publish([|msg|])
  let publishOneWithOptions (Topic topic) (msg : 'a) opts = topic.publish([|msg|], opts)
  let publish (Topic topic) (msg : 'a[]) = topic.publish(msg)
  let publishWithOptions (Topic topic) (msg : 'a[]) opts = topic.publish(msg, opts)
  let subscribeAnonymous (Topic topic) = topic.subscribe() |> Promise.map (fun (s,x) -> Subscription s, x)
  let subscribeAnonymousWithOptions (Topic topic) opts = topic.subscribe(options=opts) |> Promise.map (fun (s,x) -> Subscription s, x)
  let subscribe (Topic topic) sn = topic.subscribe(subName=sn) |> Promise.map (fun (s,x) -> Subscription s, x)
  let subscribeWithOptions (Topic topic) sn opts = topic.subscribe(sn, opts) |> Promise.map (fun (s,x) -> Subscription s, x)

module Subscription =
  let ackOne (Subscription sub) ackId = sub.ack([|ackId|])
  let ack (Subscription sub) ackIds = sub.ack(ackIds)
  let pull (Subscription sub) = sub.pull()
  let pullWithOptions (Subscription sub) opts = sub.pull(opts)
