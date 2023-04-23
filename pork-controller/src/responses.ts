export type PorkResponseType = "failure" | "eval" | "hook"

export type PorkResponse = {
  type: PorkResponseType
  flowId?: string
}

export type PorkFailureResponse = PorkResponse & {
  type: "failure"
  message: string
}

export const failureResponse = (
  message: string,
  flowId?: string
): PorkFailureResponse => ({
  type: "failure",
  message,
  flowId,
})
