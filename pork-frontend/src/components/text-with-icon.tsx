import { ReactNode } from "react"

export default function TextWithIcon({
  children,
  icon,
}: {
  children: ReactNode
  icon: ReactNode
}) {
  return (
    <div className="flex flex-row items-center gap-1">
      {icon}
      {children}
    </div>
  )
}
