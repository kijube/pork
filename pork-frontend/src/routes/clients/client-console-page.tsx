import moment from "moment"
import { useParams } from "react-router-dom"
import {
    InternalEvalRequest,
    InternalEvalResponse,
    InternalHookResponse,
    useGetClientConsoleEventsQuery,
    useRunClientEvalMutation,
} from "../../store/api"

import { ClientConsoleEvent } from "../../store/enhanced-api"
import { ScrollContainerContext, timestampFormat } from "../../utils"
import {
    ReactNode,
    useCallback,
    useContext,
    useEffect,
    useRef,
    useState,
} from "react"
import {
    ArrowLeftIcon,
    ArrowRightIcon,
    InfoIcon,
    RefreshCwIcon,
} from "lucide-react"

export default function ClientConsolePage() {
    const { clientId } = useParams()
    const { data: events } = useGetClientConsoleEventsQuery(
        { localClientId: Number(clientId!), count: 100, offset: 0 },
        { skip: !clientId, pollingInterval: 5000 }
    )
    const scrollContext = useContext(ScrollContainerContext)

    useEffect(() => {
        const ref = scrollContext?.current
        if (!ref) return
        ref.scroll({ behavior: "smooth", top: ref.scrollHeight })
    }, [events])

    return (
        <div className="flex w-full flex-col break-all px-2 text-base">
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
    const [evalCode, { isLoading }] = useRunClientEvalMutation()
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
                    localClientId: Number(clientId!),
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
            className="mt-4 w-full resize-none border-t border-neutral-700 bg-transparent p-1 px-2 font-mono placeholder-neutral-500 focus-within:border-neutral-500 focus-within:outline-none"
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
        case "hook":
            const hook = event as InternalHookResponse
            if (hook.method.startsWith("console.")) {
                return <HookConsoleEvent event={hook} />
            }
        default:
            return <div>{JSON.stringify(event)}</div>
    }
}

function HookConsoleEvent({ event }: { event: InternalHookResponse }) {
    return (
        <ConsoleEntry timestamp={event.timestamp}>
            <div className="flex flex-row items-start">
                <div className="mr-4 grid h-6 w-6 place-items-center">
                    <InfoIcon size={14} className=" text-neutral-500" />
                </div>
                <pre className="break-all">
                    {event.method.replace("console.", "")}{" "}
                    {event.args &&
                        JSON.parse(event.args)
                            .map((a: any) => JSON.stringify(a))
                            .join(" ")}
                </pre>
            </div>
        </ConsoleEntry>
    )
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
                            <ArrowLeftIcon
                                size={14}
                                className="text-neutral-500"
                            />
                        ) : (
                            <RefreshCwIcon size={14} className="animate-spin" />
                        )}
                    </div>
                    <pre className="flex-1 break-words text-neutral-400">
                        {event.code}
                    </pre>
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
            <div className="ml-4 flex w-24 flex-col pt-1 text-xs text-neutral-500">
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
                    <ArrowRightIcon size={14} className=" text-neutral-500" />
                </div>
                <pre className="break-all">
                    {event.data.substring(1, event.data.length - 1)}
                </pre>
            </div>
        </ConsoleEntry>
    )
}
