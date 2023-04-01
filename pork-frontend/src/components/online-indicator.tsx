export const onlineBgColor = "bg-green-500"
export const offlineBgColor = "bg-red-500"
export const onlineTextColor = "text-green-700"
export const offlineTextColor = "text-red-700"

export default function OnlineIndicator({
  isOnline,
  className = "",
}: {
  isOnline?: boolean
  className?: string
}) {
  const color = isOnline ? onlineBgColor : offlineBgColor
  return (
    <div
      className={`h-3 w-3 ${className} rounded-full ${color} grid place-items-center`}
    >
      {isOnline && (
        <div className={`h-2/3 w-2/3 animate-ping rounded-full ${color}`}></div>
      )}
    </div>
  )
}
