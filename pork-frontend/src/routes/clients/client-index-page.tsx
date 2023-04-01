import moment from "moment"
import { useState } from "react"
import { Outlet, useParams } from "react-router-dom"
import OnlineIndicator from "../../components/online-indicator"
import { useGetClientLogsQuery, useGetClientQuery } from "../../store/api"
import {
  InformationCircleIcon,
  ExclamationCircleIcon,
} from "@heroicons/react/20/solid"
import Nickname from "../../components/nickname"
import ClientTabs from "../../components/client-tabs"

export default function ClientIndexPage() {
  const { clientId } = useParams()
  const {
    data: client,
    isLoading,
    isSuccess,
  } = useGetClientQuery({ clientId: clientId! }, { pollingInterval: 3000 })
  console.log(client)
  return (
    <div className="flex min-h-full flex-col items-stretch">
      <nav className="sticky top-0 z-50 w-full bg-neutral-900 p-2 pb-0">
        <div className="flex w-full flex-row items-center gap-2 border-b border-neutral-700 px-2 pb-1">
          <OnlineIndicator className="h-4 w-4" isOnline={client?.isOnline} />
          <h1 className="text-2xl font-bold">
            {isSuccess ? client.remoteIp : "..."}
          </h1>
          <Nickname clientId={clientId} />
          {isSuccess && (
            <>
              <span className="text-xs text-neutral-600">
                last seen {moment(client.lastSeen).format("DD.MM. HH:mm:ss")}
              </span>
            </>
          )}
          <ClientTabs clientId={clientId} />
        </div>
      </nav>
      <Outlet />
    </div>
  )
}
