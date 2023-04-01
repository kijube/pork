import { useLocation, useNavigate, useParams } from "react-router-dom"

type Tab = {
  path: string
  name: string
}

const tabs: Tab[] = [
  { name: "Dashboard", path: "/" },
  { name: "Logs", path: "/logs" },
]

export default function ClientTabs({ clientId }: { clientId?: string }) {
  const navigate = useNavigate()
  const location = useLocation()
  const base = `/clients/${clientId}`

  if (!clientId) {
    return <></>
  }

  return (
    <div className="flex flex-row gap-1">
      {tabs.map((tab) => {
        return (
          <div
            key={tab.path}
            className={`flex cursor-pointer flex-row items-center gap-1 px-2 py-1 ${
              location.pathname === `${base}${tab.path}`
                ? "underline"
                : "hover:underline"
            }`}
            onClick={() => navigate(`${base}${tab.path}`)}
          >
            {tab.name}
          </div>
        )
      })}
    </div>
  )
}
