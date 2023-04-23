import {
    ReactNode,
    useCallback,
    useContext,
    useEffect,
    useRef,
    useState,
} from "react"
import { useParams } from "react-router-dom"
import {
    InternalEvalRequest,
    InternalEvalResponse,
    InternalSiteBroadcastMessage,
    useBroadcastEvalMutation,
    useGetSiteEvalsQuery,
    useRunClientEvalMutation,
} from "../../store/api"
import { useGetSiteKey } from "../../hooks"
import { ScrollContainerContext, timestampFormat } from "../../utils"
import moment from "moment"
import { ArrowLeftIcon, RefreshCwIcon } from "lucide-react"

export default function SiteBroadcastConsolePage() {
    const siteKey = useGetSiteKey()
    const { data: events } = useGetSiteEvalsQuery(
        { siteKey: siteKey!, count: 100, offset: 0 },
        { skip: !siteKey, pollingInterval: 5000 }
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
                    <BroadcastEvalEvent event={ev} />
                </div>
            ))}
            <ConsoleInput />
        </div>
    )
}

function ConsoleInput() {
    const siteKey = useGetSiteKey()
    const [evalCode, { isLoading }] = useBroadcastEvalMutation()
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
            if (!siteKey) return
            if (e.key === "Enter" && !e.shiftKey) {
                e.preventDefault()
                evalCode({
                    broadcastEvalRequestDto: { code: e.currentTarget.value },
                    siteKey: siteKey,
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

function BroadcastEvalEvent({
    event,
}: {
    event: InternalSiteBroadcastMessage
}) {
    return (
        <>
            <ConsoleEntry timestamp={event.timestamp}>
                <div className="flex flex-row items-start">
                    <div className="mr-4 grid h-6 w-6 place-items-center">
                        <ArrowLeftIcon size={14} className="text-neutral-500" />
                    </div>
                    <pre className="flex-1 break-words text-neutral-400">
                        {event.code}
                    </pre>
                </div>
            </ConsoleEntry>
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
