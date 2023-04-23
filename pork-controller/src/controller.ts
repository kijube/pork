import { pork } from "./pork"
import { evalRequest } from "./requests/eval-request"

pork.register("eval", evalRequest)

pork.connect()
