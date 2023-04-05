import moment from "moment"
import { useParams } from "react-router-dom"
import {
    InternalEvalRequest,
    InternalEvalResponse,
    useClientEvalMutation,
    useGetClientConsoleEventsQuery,
} from "../../store/api"
import { ArrowPathIcon } from "@heroicons/react/24/solid"
import { ClientConsoleEvent } from "../../store/enhanced-api"
import { timestampFormat } from "../../utils"

export default function ClientConsolePage() {
    const { clientId } = useParams()
    const { data: events } = useGetClientConsoleEventsQuery(
        { clientId: clientId!, count: 100, offset: 0 },
        { skip: !clientId, pollingInterval: 5000 }
    )
    return (
        <div className="flex w-full flex-col gap-2 break-all px-2 pt-4 text-base">
            {events?.map((ev, i) => (
                <div
                    key={"" + ev.flowId + ev.timestamp}
                    className="odd:bg-neutral-800 odd:bg-opacity-25"
                >
                    <ConsoleEvent event={ev} />
                </div>
            ))}
            <CodeInput />
        </div>
    )
}

function CodeInput() {
    const { clientId } = useParams()
    const [evalCode, { isLoading }] = useClientEvalMutation()

    function handleKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
        if (!clientId) return
        if (e.key === "Enter" && e.ctrlKey) {
            e.preventDefault()
            evalCode({
                clientId: clientId!,
                evalRequestDto: { code: e.currentTarget.value },
            })
            e.currentTarget.value = ""
        }
    }

    return (
        <textarea
            className="mt-4 w-full resize-none rounded border border-neutral-700 bg-transparent p-1 px-2 font-mono text-sm focus-within:border-neutral-500 focus-within:outline-none"
            placeholder="Enter JavaScript code..."
            disabled={isLoading}
            onKeyDown={handleKeyDown}
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
            <div className="flex flex-row items-start">
                <div className="mr-4 w-24 pt-1 text-xs text-neutral-500">
                    {moment(event.timestamp).format(timestampFormat)}
                </div>
                <div className="flex-1">
                    <div className="flex flex-row items-center">
                        {!event.response && (
                            <ArrowPathIcon className="h-4 w-4" />
                        )}
                        <pre className="flex-1">{event.code}</pre>
                    </div>
                </div>
            </div>
            {response}
        </>
    )
}

function EvalResponseEvent({ event }: { event?: InternalEvalResponse }) {
    if (!event) {
        return <></>
    }
    return (
        <div className="flex flex-row items-start">
            <div className="mr-4 w-24 pt-1 text-xs text-neutral-500">
                {moment(event.timestamp).format(timestampFormat)}
            </div>
            <div className="flex-1">
                <code>{event.data}</code>
            </div>
        </div>
    )
}
