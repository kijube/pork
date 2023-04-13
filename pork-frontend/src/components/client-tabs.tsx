import { useCallback } from "react"
import { useLocation, useNavigate, useParams } from "react-router-dom"

type Tab = {
    path: string
    name: string
    key: string
}

const tabs: Tab[] = [
    { name: "Dashboard", path: "", key: "D" },
    { name: "Console", path: "/console", key: "C" },
    { name: "Logs", path: "/logs", key: "L" },
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
                                ? "text-neutral-200"
                                : "text-neutral-400 hover:text-neutral-200"
                        } transition`}
                        onClick={() => navigate(`${base}${tab.path}`)}
                    >
                        <KeyedName name={tab.name} letter={tab.key} />
                    </div>
                )
            })}
        </div>
    )
}

function KeyedName({ name, letter: key }: { name: string; letter: string }) {
    const keyIdx = name.toLowerCase().indexOf(key.toLowerCase())
    const split = [
        name.slice(0, keyIdx),
        name.slice(keyIdx, keyIdx + key.length),
    ]
    return (
        <span>
            {split[0]}
            <span className="underline ">{split[1]}</span>
            {name.slice(keyIdx + key.length)}
        </span>
    )
}
