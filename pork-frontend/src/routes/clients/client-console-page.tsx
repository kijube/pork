import moment from "moment"
import { useParams } from "react-router-dom"
import {
  InternalEvalRequest,
  InternalEvalResponse,
  useClientEvalMutation,
  useGetClientConsoleEventsQuery,
} from "../../store/api"
import {
  ArrowLeftIcon,
  ArrowPathIcon,
  ArrowRightIcon,
  ChevronDoubleLeftIcon,
} from "@heroicons/react/20/solid"
import { ClientConsoleEvent } from "../../store/enhanced-api"
import { timestampFormat } from "../../utils"
import {
  ReactNode,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
} from "react"
import { ScrollContainerContext } from "./client-index-page"

export default function ClientConsolePage() {
  const { clientId } = useParams()
  const { data: events } = useGetClientConsoleEventsQuery(
    { clientId: clientId!, count: 100, offset: 0 },
    { skip: !clientId, pollingInterval: 5000 }
  )
  const scrollContext = useContext(ScrollContainerContext)

  useEffect(() => {
    const ref = scrollContext?.current
    if (!ref) return
    ref.scroll({ behavior: "smooth", top: ref.scrollHeight })
  }, [events])

  return (
    <div className="flex w-full flex-col break-all px-2 pt-4 text-base">
      {events?.map((ev, i) => (
        <div
          key={"" + ev.flowId + ev.timestamp}
          className="px-4 py-2 odd:bg-neutral-800 odd:bg-opacity-25"
        >
          <ConsoleEvent event={ev} />
        </div>
      ))}
      <ConsoleInput />
    </div>
  )
}

function ConsoleInput() {
  const { clientId } = useParams()
  const [evalCode, { isLoading }] = useClientEvalMutation()
  const [rows, setRows] = useState(2)
  const ref = useRef<HTMLTextAreaElement>(null)

  const updateRows = useCallback(
    (content: string) => {
      if (!content || content.length === 0) {
        setRows(2)
        return
      }
      const lines = content.split(/\r\n|\r|\n/).length
      setRows(Math.max(2, lines))
    },
    [setRows]
  )

  const handleKeyDown = useCallback(
    (e: React.KeyboardEvent<HTMLTextAreaElement>) => {
      if (!clientId) return
      if (e.key === "Enter" && !e.shiftKey) {
        e.preventDefault()
        evalCode({
          clientId: clientId!,
          evalRequestDto: { code: e.currentTarget.value },
        })
        e.currentTarget.value = ""
        setTimeout(() => {
          ref?.current?.focus()
        }, 50)
      }
    },
    [evalCode]
  )

  useEffect(() => {
    ref?.current?.focus()
  }, [])

  return (
    <textarea
      ref={ref}
      rows={rows}
      className="mt-4 w-full resize-none border-t border-neutral-700 bg-transparent p-1 px-2 font-mono focus-within:border-neutral-500 focus-within:outline-none"
      placeholder="Enter JavaScript code..."
      disabled={isLoading}
      onKeyDown={handleKeyDown}
      onChange={(e) => updateRows(e.currentTarget.value)}
    ></textarea>
  )
}

function ConsoleEvent({ event }: { event: ClientConsoleEvent }) {
  switch (event.type) {
    case "evalreq":
      return <EvalRequestEvent event={event as InternalEvalRequest} />
    default:
      return <div>{JSON.stringify(event)}</div>
  }
}

function EvalRequestEvent({ event }: { event: InternalEvalRequest }) {
  const response = EvalResponseEvent({
    event: event.response as InternalEvalResponse | undefined,
  })
  return (
    <>
      <ConsoleEntry timestamp={event.timestamp}>
        <div className="flex flex-row items-start">
          <div className="mr-4 grid h-6 w-6 place-items-center">
            {event.sent ? (
              <ArrowLeftIcon className="h-4 w-4 text-neutral-500" />
            ) : (
              <ArrowPathIcon className="h-4 w-4 animate-spin" />
            )}
          </div>
          <pre className="flex-1 text-neutral-400 break-words">{event.code}</pre>
        </div>
      </ConsoleEntry>
      {response}
    </>
  )
}

function ConsoleEntry({
  children,
  timestamp,
}: {
  children: ReactNode
  timestamp: string
}) {
  return (
    <div className="flex flex-row items-start">
      <div className="flex-1">{children}</div>
      <div className="ml-4 w-24 flex flex-col pt-1 text-xs text-neutral-500">
        {moment(timestamp).format(timestampFormat)}
      </div>
    </div>
  )
}

function EvalResponseEvent({ event }: { event?: InternalEvalResponse }) {
  if (!event) {
    return <></>
  }
  return (
    <ConsoleEntry timestamp={event.timestamp}>
      <div className="flex flex-row items-start">
        <div className="mr-4 grid h-6 w-6 place-items-center">
          <ArrowRightIcon className="h-4 w-4 text-neutral-500" />
        </div>
        <pre className="break-all">{event.data.substring(1, event.data.length-1)}</pre>
      </div>
    </ConsoleEntry>
  )
}
