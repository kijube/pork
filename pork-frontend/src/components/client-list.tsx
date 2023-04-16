import { Link, useParams } from "react-router-dom"
import { LocalClientDto, useGetClientsQuery } from "../store/api"
import OnlineIndicator from "./online-indicator"

export default function ClientList() {
  const { data, isLoading, isSuccess } = useGetClientsQuery(
    {} as unknown as void,
    {
      pollingInterval: 3000,
    }
  )
  if (isLoading) {
    return <div>loading...</div>
  }

  if (!isSuccess) {
    return <div>loading failed.</div>
  }

  return (
    <ul className="flex flex-col gap-1">
      {data.map((c) => {
        return <ClientListItem key={c.id} client={c}></ClientListItem>
      })}
    </ul>
  )
}

function ClientListItem({ client }: { client: LocalClientDto }) {
  const { clientId } = useParams()
  const color = client.isOnline ? "bg-green-500" : "bg-red-500"
  const isActive = Number(clientId) === client.id

  return (
    <Link to={`/clients/${client.id}`}>
      <li
        className={`${
          isActive && color
        } flex flex-row items-center gap-1 rounded ${
          !isActive && "hover:underline"
        }`}
      >
        <OnlineIndicator isOnline={client.isOnline} />
        {client.globalClient.nickname ? client.globalClient.nickname : client.globalClient.remoteIp}
      </li>
    </Link>
  )
}
