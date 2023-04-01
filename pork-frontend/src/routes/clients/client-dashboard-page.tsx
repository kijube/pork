import moment from "moment"
import { useParams } from "react-router-dom"
import { useGetClientEventsQuery } from "../../store/api"

export default function ClientDashboardPage() {
  return (
    <main className="grid flex-1 auto-rows-auto grid-cols-2 gap-2 p-2">
      <div className="relative col-span-1 h-full overflow-y-scroll rounded border border-neutral-700 text-sm">
        <div className="absolute h-full w-full">
          <Evaluator />
        </div>
      </div>
      <div className="relative col-span-1 h-full overflow-y-scroll rounded border border-neutral-700 text-sm">
        <div className="absolute h-full w-full">
          <EventLog />
        </div>
      </div>
      <div className="col-span-1 h-full overflow-y-scroll rounded border border-neutral-700 text-sm"></div>
      <div className="relative col-span-1 h-full overflow-y-scroll rounded border border-neutral-700 text-sm">
        <div className="absolute h-full w-full">toaster</div>
      </div>
    </main>
  )
}

function Evaluator() {
  return (
    <div className="min-h-full w-full p-2">
      <textarea className="w-full resize-none rounded bg-neutral-800 p-1 focus-within:outline-none"></textarea>
    </div>
  )
}

function EventLog() {
  const { clientId } = useParams()
  const {
    data: events,
    isLoading,
    isSuccess,
  } = useGetClientEventsQuery(
    { clientId: clientId!, count: 50, offset: 0 },
    { skip: !clientId, pollingInterval: 3000 }
  )

  if (isLoading) {
    return <div>loading...</div>
  }

  if (!isSuccess) {
    return <div>error</div>
  }

  return (
    <div className="min-h-full w-full">
      <table className="w-full">
        <tbody>
          {events.map((e, i) => {
            return (
              <tr
                key={e.timestamp + "" + i}
                className={` odd:bg-neutral-800 odd:bg-opacity-25`}
              >
                <td className="py-1 pl-4">
                  {e.event?.type}
                </td>
                <td className="py-1 pl-4 opacity-50">
                  {moment(e.timestamp).format("DD.MM. HH:mm:ss")}
                </td>
                <td className="py-1 pl-8 pr-4">{JSON.stringify(e.event)}</td>
              </tr>
            )
          })}
        </tbody>
      </table>
    </div>
  )
}
