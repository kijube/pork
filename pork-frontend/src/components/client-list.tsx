import { Link, useParams } from "react-router-dom"
import { LocalClientDto, SiteDto, useGetSitesQuery } from "../store/api"
import OnlineIndicator from "./online-indicator"

export default function ClientList() {
  const { data, isLoading, isSuccess } = useGetSitesQuery(undefined as any, {
    pollingInterval: 3000,
  })
  if (isLoading) {
    return <div>loading...</div>
  }

  if (!isSuccess) {
    return <div>loading failed.</div>
  }

  return (
    <ol className="flex flex-col gap-1">
      {data.map((s) => {
        return <SiteListItem key={s.key} site={s}></SiteListItem>
      })}
    </ol>
  )
}

function SiteListItem({ site }: { site: SiteDto }) {
  return (
    <li>
      <h4 className="font-bold">{site.key}</h4>
      <ol>
        {site.localClients.map((c) => {
          return <ClientListItem key={c.id} client={c}></ClientListItem>
        })}
      </ol>
    </li>
  )
}

function ClientListItem({ client }: { client: LocalClientDto }) {
  const { clientId } = useParams()
  const color = client.isOnline ? "bg-green-500" : "bg-red-500"
  const isActive = Number(clientId) === client.id

  return (
    <Link to={`/sites/${client.site.key}/clients/${client.id}`}>
      <li
        className={`${
          isActive && color
        } flex flex-row items-center gap-1 rounded ${
          !isActive && "hover:underline"
        }`}
      >
        <OnlineIndicator isOnline={client.isOnline} />
        {client.globalClient.nickname
          ? client.globalClient.nickname
          : client.remoteIp}
      </li>
    </Link>
  )
}
