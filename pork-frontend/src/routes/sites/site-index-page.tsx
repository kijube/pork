import { Outlet } from "react-router-dom"
import { useGetCurrentSite } from "../../hooks"
import { useMemo, useRef } from "react"
import Tabs, { TabProps } from "../../components/tabs"
import TextWithIcon from "../../components/text-with-icon"
import { ChevronRightSquareIcon, LayoutTemplateIcon } from "lucide-react"
import { ScrollContainerContext } from "../../utils"

export default function SiteIndexPage() {
    const { data: site, siteKey } = useGetCurrentSite()
    const ref = useRef<HTMLDivElement>(null)

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
                {
                    name: (
                        <TextWithIcon
                            icon={<ChevronRightSquareIcon size={14} />}
                        >
                            Broadcast
                        </TextWithIcon>
                    ),
                    path: "/broadcast",
                },
            ],
        }),
        [siteKey]
    )
    return (
        <div className="flex h-full w-full flex-col gap-2">
            <nav className="flex flex-row gap-2 border-b border-neutral-700 p-2 px-4">
                <h1 className="text-2xl font-bold">
                    {site ? <span>{site.key}</span> : <span>...</span>}
                </h1>
                <div className="flex-1"></div>
                <Tabs {...tabProps} />
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
