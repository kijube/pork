import moment from "moment"
import { useParams } from "react-router-dom"
import { useGetLocalClientEventsQuery } from "../../store/api"

export default function ClientDashboardPage() {
  return (
    <main className="grid min-h-full w-full auto-rows-auto grid-cols-5 gap-2 p-2 pt-0">
      <div className="relative col-span-2 h-full overflow-y-scroll rounded border border-neutral-700 text-sm">
        <div className="absolute h-full w-full">
          <Evaluator />
        </div>
      </div>
      <div className="relative col-span-3 h-full overflow-y-scroll rounded border border-neutral-700 text-sm">
        <div className="absolute h-full w-full">
          <EventLog />
        </div>
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
  } = useGetLocalClientEventsQuery(
    { localClientId: Number(clientId!), count: 50, offset: 0 },
    { skip: !clientId, pollingInterval: 3000 }
  )

  if (isLoading) {
    return <div>loading...</div>
  }

  if (!isSuccess) {
    return <div>error</div>
  }

  return (
      <table className="max-w-full w-full">
        <tbody>
          {events.map((e, i) => {
            return (
              <tr
                key={e.timestamp + "" + i}
                className={` odd:bg-neutral-800 odd:bg-opacity-25`}
              >
                <td className="py-1 pl-4">{e.type}</td>
                <td className="py-1 pl-4 opacity-50">
                  {moment(e.timestamp).format("DD.MM. HH:mm:ss")}
                </td>
                <td className="py-1 pl-8 pr-4 break-all">{JSON.stringify(e)}</td>
              </tr>
            )
          })}
        </tbody>
      </table>
  )
}
