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

type GetSnapshotsOptions =
  { autoPaginate: bool option
    maxApiCalls: int option
    maxResults: int option
    pageSize: int option
    pageToken: string option
  }

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

module pubsub_types =
  type ApiResponse = interface end

  type Subscription =
    abstract ack: [<ParamArray>] ackIds: AckId[] * ?options: AckOptions -> JS.Promise<unit>
    abstract on: eventType:string -> listener:('a -> unit) -> unit
    abstract removeListener: eventType: string -> listener: ('a -> unit) -> unit
    abstract pull: ?options:PullOptions -> JS.Promise<Message[] * ApiResponse>

  type Topic =
    abstract exists: unit -> JS.Promise<bool * ApiResponse>
    abstract get: ?options:TopicGetOptions -> JS.Promise<Topic * ApiResponse>
    abstract publish: [<ParamArray>] message: 'a[] * ?options: PublishOptions -> JS.Promise<MessageId[] * ApiResponse>
    abstract subscribe: ?subName: SubscriptionName * ?options: SubscribeOptions -> JS.Promise<Subscription * ApiResponse>

  type PubSub =
    abstract createTopic: topicName: TopicName -> JS.Promise<Topic * ApiResponse>
    abstract subscribe: topic: U2<Topic,TopicName> * ?subName: SubscriptionName * ?options: SubscribeOptions -> JS.Promise<Subscription * ApiResponse>
    abstract subscription: ?name: SubscriptionName * ?options: SubscriptionOptions -> Subscription
    abstract topic: name: TopicName -> Topic

  type Globals =
    [<Emit("$0($1)")>]
    abstract Init : ?options:Options -> PubSub

[<Import("default","@google-cloud/pubsub")>]
let pubsub: pubsub_types.Globals = jsNative
