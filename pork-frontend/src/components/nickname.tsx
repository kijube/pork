import { useRef, useState } from "react"
import {
  useGetLocalClientByIdQuery,
  useSetClientNicknameMutation,
} from "../store/api"
import { useGetCurrentLocalClient } from "../hooks"

export default function Nickname() {
  const { data, clientId, isLoading, isSuccess } = useGetCurrentLocalClient()

  const ref = useRef(null)

  const [isEditing, setIsEditing] = useState(false)
  const [currentValue, setCurrentValue] = useState("")

  const [setNickname, { isLoading: isSetting }] = useSetClientNicknameMutation()

  function edit() {
    setIsEditing(true)
    setCurrentValue(data?.globalClient?.nickname ?? "")
    setTimeout(() => {
      ;(ref.current as any).focus()
      ;(ref.current as any).setSelectionRange(0, 9999)
    }, 50)
  }

  function save() {
    if (!clientId || !data) {
      return
    }
    setIsEditing(false)
    setNickname({
      globalClientId: data.globalClient.id,
      setNicknameRequestDto: { nickname: currentValue },
    })
  }

  if (!clientId || isLoading || isSetting) {
    return <div>loading...</div>
  }

  if (!isSuccess) {
    return <div>error</div>
  }

  if (isEditing) {
    return (
      <div className="flex flex-row items-center text-sm">
        <input
          ref={ref}
          className="w-32 rounded bg-neutral-800 px-1 focus-within:outline-none"
          type="text"
          value={currentValue}
          onChange={(e) => setCurrentValue(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              save()
            }

            if (e.key === "Escape") {
              setIsEditing(false)
            }
          }}
        />
        <button
          className="pl-1 text-xs text-neutral-500 hover:underline"
          onClick={() => setIsEditing(false)}
        >
          cancel
        </button>
        <button
          className="pl-0.5 text-xs text-neutral-500 hover:underline"
          onClick={save}
        >
          save
        </button>
      </div>
    )
  }

  return (
    <button onClick={edit} className="group text-sm">
      (
      <span className="group-hover:underline">
        {data.globalClient?.nickname
          ? data.globalClient.nickname
          : "set nickname"}
      </span>
      )
    </button>
  )
}
