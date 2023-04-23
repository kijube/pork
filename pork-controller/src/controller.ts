import { pork } from "./pork"
import { evalRequest } from "./features/requests/eval-request"
pork.register("eval", evalRequest)

pork.connect()
