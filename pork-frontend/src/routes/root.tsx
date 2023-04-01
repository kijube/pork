import { Outlet } from "react-router-dom"
import Sidebar from "../components/sidebar"

export default function Root() {
  return (
    <div className="flex min-h-screen flex-row">
      <Sidebar />
      <div className="flex-1">
        <Outlet />
      </div>
    </div>
  )
}
