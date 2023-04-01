import ClientList from "./client-list"

export default function Sidebar() {
  return (
    <>
      <div className="h-full w-64"></div>
      <div className="fixed h-screen w-64 border-r border-r-neutral-700 bg-neutral-900 p-2">
        <ClientList />
      </div>
    </>
  )
}
