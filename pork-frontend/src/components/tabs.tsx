import { ReactNode } from "react"
import { useMatch, useNavigate } from "react-router-dom"

export type Tab = {
  path: string
  name: ReactNode
}

export type TabProps = {
  tabs: Tab[]
  rootPath: string
}

export default function Tabs({ tabs, rootPath }: TabProps) {
  return (
    <div className="flex flex-row gap-1">
      {tabs.map((tab) => {
        return (
          <TabDisplay key={tab.path} tab={tab} rootPath={rootPath}></TabDisplay>
        )
      })}
    </div>
  )
}

function TabDisplay({ tab, rootPath }: { tab: Tab; rootPath: string }) {
  const navigate = useNavigate()
  const isMatch = useMatch(rootPath + tab.path)
  return (
    <div
      className={`flex cursor-pointer flex-row items-center gap-1 px-2 py-1 ${
        isMatch ? "text-neutral-200" : "text-neutral-400 hover:text-neutral-200"
      } transition`}
      onClick={() => navigate(`${rootPath}${tab.path}`)}
    >
      {tab.name}
    </div>
  )
}
