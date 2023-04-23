import { pork } from "../pork"
import { PorkResponse, failureResponse } from "../responses"
import { PorkRequest, PorkRequestHandler } from "./request"

export type PorkEvalRequest = PorkRequest & {
  type: "eval"
  code: string
}

export type PorkEvalResponse = PorkResponse & {
  type: "eval"
  data: any
}

export const evalRequest: PorkRequestHandler<PorkEvalRequest> = (request) => {
  const execCode = `try {${request.code}}catch(e){e.toString()}`

  Promise.resolve(eval(execCode))
    .then((data) => {
      pork.respond({
        type: "eval",
        flowId: request.flowId,
        data: `${data}`,
      } as PorkEvalResponse)
    })
    .catch((e) => {
      pork.respond(failureResponse(e.toString(), request.flowId))
    })
}
