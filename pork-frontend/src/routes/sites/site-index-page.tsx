import { Outlet } from "react-router-dom"
import { useGetCurrentSite } from "../../hooks"
import { useMemo } from "react"
import Tabs, { TabProps } from "../../components/tabs"
import TextWithIcon from "../../components/text-with-icon"
import { LayoutTemplateIcon } from "lucide-react"

export default function SiteIndexPage() {
  const { data: site, siteKey } = useGetCurrentSite()
  const tabProps = useMemo<TabProps>(
    () => ({
      rootPath: `/sites/${siteKey}`,
      tabs: [
        {
          name: (
            <TextWithIcon icon={<LayoutTemplateIcon size={14} />}>
              Dashboard
            </TextWithIcon>
          ),
          path: "",
        },
      ],
    }),
    [siteKey]
  )
  return (
    <div className="flex h-full w-full flex-col">
      <nav className="flex flex-row gap-2 border-b border-neutral-700 p-2 px-4">
        <h1 className="text-2xl font-bold">
          {site ? <span>{site.key}</span> : <span>...</span>}
        </h1>
        <div className="flex-1"></div>
        <Tabs {...tabProps} />
      </nav>
      <div className="flex-1">
        <Outlet />
      </div>
    </div>
  )
}
