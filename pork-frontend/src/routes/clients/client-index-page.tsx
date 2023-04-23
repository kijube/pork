import moment from "moment"
import { Outlet } from "react-router-dom"
import OnlineIndicator from "../../components/online-indicator"
import Nickname from "../../components/nickname"
import Tabs, { TabProps } from "../../components/tabs"
import { ScrollContainerContext, timestampFormat } from "../../utils"
import { createContext, RefObject, useMemo, useRef } from "react"
import { useGetCurrentLocalClient, useGetSiteKey } from "../../hooks"
import { ChevronRightSquareIcon, ScrollIcon, UserIcon } from "lucide-react"
import TextWithIcon from "../../components/text-with-icon"



export default function ClientIndexPage() {
  const { data: client, isSuccess, clientId } = useGetCurrentLocalClient()
  const siteKey = useGetSiteKey()
  const ref = useRef<HTMLDivElement>(null)
  const tabProps = useMemo<TabProps>(
    () => ({
      rootPath: `/sites/${siteKey}/clients/${clientId}`,
      tabs: [
        {
          name: <TextWithIcon icon={<UserIcon size={14} />}>Client</TextWithIcon>,
          path: "",
        },
        {
          name: (
            <TextWithIcon icon={<ChevronRightSquareIcon size={14} />}>
              Console
            </TextWithIcon>
          ),
          path: "/console",
        },
        {
          name: (
            <TextWithIcon icon={<ScrollIcon size={14} />}>Logs</TextWithIcon>
          ),
          path: "/logs",
        },
      ],
    }),
    [siteKey, clientId]
  )
  return (
    <div className="flex h-full flex-col items-stretch gap-2">
      <nav className="w-full bg-neutral-900 px-2">
        <div className="flex w-full flex-row items-center gap-2 px-2">
          <OnlineIndicator className="h-4 w-4" isOnline={client?.isOnline} />
          <h1 className="text-xl font-bold">
            {isSuccess ? (client.remoteIp ?? "[unknown ip]") : "..."}
          </h1>
          <Nickname />
          {isSuccess && (
            <>
              <span className="text-xs text-neutral-600">
                last seen {moment(client.lastSeen).format(timestampFormat)}
              </span>
            </>
          )}
          <div className="flex-1"></div>
          <Tabs {...tabProps} />
        </div>
      </nav>
      <ScrollContainerContext.Provider value={ref}>
        <div ref={ref} className="relative flex-1 overflow-y-scroll">
          <div className="absolute top-0 bottom-0 left-0 right-0">
            <Outlet />
          </div>
        </div>
      </ScrollContainerContext.Provider>
    </div>
  )
}
