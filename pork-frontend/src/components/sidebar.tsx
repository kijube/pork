import { PiggyBankIcon } from "lucide-react"
import ClientList from "./client-list"

export default function Sidebar() {
    return (
        <>
            <div className="h-full w-48"></div>
            <div className="fixed h-screen w-48 border-r border-r-neutral-700 bg-neutral-900 p-2 overflow-y-scroll">
                <ClientList />
            </div>
        </>
    )
}
