import { pork } from "../../pork"
import { PorkResponse } from "../../responses"

type DumpResponse = PorkResponse & {
  type: "dump"
  dump: string
  key: string
}

export const dumpResponse = (key: string, obj: object) =>
  ({
    type: "dump",
    key,
    dump: JSON.stringify(obj),
  } as DumpResponse)
