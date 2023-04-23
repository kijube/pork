export type PorkRequestType = "eval"
export type PorkRequest = {
  type: PorkRequestType
  flowId?: string
}

export type BasePorkRequestHandler = (request: any) => void

export type PorkRequestHandler<T extends PorkRequest> = (request: T) => void
