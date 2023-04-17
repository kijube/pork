import moment from "moment"
import { useState } from "react"
import { useParams } from "react-router-dom"
import { useGetClientLogsQuery } from "../../store/api"
import { useGetClientId } from "../../hooks"

export default function ClientLogsPage() {
  return (
    <div className="px-2 text-sm">
      <Logs />
    </div>
  )
}

function Logs() {
  const clientId = useGetClientId()
  const [offset, setOffset] = useState(0)
  const {
    data: logs,
    isLoading,
    isSuccess,
  } = useGetClientLogsQuery(
    { localClientId: clientId!, count: 100, offset },
    { pollingInterval: 5000, skip: !clientId }
  )
  if (isLoading) {
    return <div>loading...</div>
  }

  if (!isSuccess) {
    return <div>error</div>
  }

  return (
    <table className="w-full">
      <tbody>
        {logs?.map((l) => {
          return (
            <tr
              key={l.timestamp + l.message}
              className={`${
                levelColors[l.level!]
              } odd:bg-neutral-800 odd:bg-opacity-25`}
            >
              <td className="py-1 pl-4">
                <LevelIndicator level={l.level!} />
              </td>
              <td className="py-1 pl-4 opacity-50">
                {moment(l.timestamp).format("DD.MM. HH:mm:ss")}
              </td>
              <td className="break-all py-1 pl-8 pr-4">{l.message}</td>
            </tr>
          )
        })}
      </tbody>
    </table>
  )
}

const levelColors = {
  Information: "text-neutral-300",
  Warning: "text-yellow-500",
  Error: "text-red-500",
} as any

function LevelIndicator({ level = "Information" }: { level: string }) {
  const classes = `h-4 w-4 ${levelColors[level] as string}`
  switch (level) {
    case "Information":
      return <span className={classes}>info</span>
    case "Warning":
      return <span className={classes}>warn</span>
    case "Error":
      return <span className={classes}>error</span>
  }

  return <div></div>
}
