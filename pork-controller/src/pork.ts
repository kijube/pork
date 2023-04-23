import {
  BasePorkRequestHandler,
  PorkRequest,
  PorkRequestHandler,
  PorkRequestType,
} from "./requests/request"
import { PorkResponse, failureResponse } from "./responses"

type Pork = {
  respond: (response: PorkResponse) => void
}

type PorkConnector = Pork & {
  connect: (autoReconnect?: boolean) => void
  disconnect: () => void
  isConnected: () => boolean
  register: <T extends PorkRequest>(
    type: PorkRequestType,
    handler: PorkRequestHandler<T>
  ) => void
}

let socket: WebSocket
let isConnected: boolean

let responseBuffer: PorkResponse[] = []
const handlers = new Map<PorkRequestType, BasePorkRequestHandler>()

const connect = (autoReconnect = true) => {
  if (socket) {
    return
  }

  let receivedId = false

  const id = localStorage.getItem("porkId")
  const idStr = id ? `?id=${id}` : ""

  socket = new WebSocket(`ws://localhost:9092/connect${idStr}`)
  socket.onopen = () => {}

  socket.onclose = () => {
    isConnected = false
    if (autoReconnect) {
      setTimeout(() => connect(), 2000)
    }
  }

  socket.onmessage = (event) => {
    // first message is always the id
    if (!receivedId) {
      isConnected = true
      receivedId = true
      localStorage.setItem("porkId", event.data)
      // send any buffered responses
      responseBuffer.forEach((response) => respond(response))
      responseBuffer = []
      return
    }

    const request = JSON.parse(event.data) as PorkRequest
    const handler = handlers.get(request.type)
    if (!handler) {
      respond(failureResponse(`Unknown request type: ${request.type}`))
      return
    }
    handler(request)
  }
}

const disconnect = () => {
  if (!socket) {
    return
  }
  socket.close()
}

const respond = (response: PorkResponse) => {
  if (!isConnected) {
    responseBuffer.push(response)
    return
  }

  socket.send(JSON.stringify(response))
}

export const pork: PorkConnector = {
  connect,
  disconnect,
  isConnected: () => isConnected,
  respond,
  register: <T extends PorkRequest>(
    type: PorkRequestType,
    handler: PorkRequestHandler<T>
  ) => handlers.set(type, handler),
}
