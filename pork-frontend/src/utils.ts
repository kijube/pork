import { RefObject, createContext } from "react"

export const timestampFormat = "DD.MM. HH:mm:ss"
export const dayFormat = "DD.MM."
export const timeFormat = "HH:mm:ss"

export const ScrollContainerContext =
  createContext<RefObject<HTMLDivElement> | null>(null)