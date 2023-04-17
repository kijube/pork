import { useParams } from "react-router-dom"
import { useGetLocalClientByIdQuery, useGetSiteByKeyQuery } from "./store/api"

export function useGetCurrentLocalClient() {
  const clientId = useGetClientId()

  return {
    clientId,
    ...useGetLocalClientByIdQuery(
      { localClientId: clientId! },
      { pollingInterval: 3000, skip: !clientId }
    ),
  }
}

export function useGetClientId() {
  const { clientId } = useParams()

  if (!clientId) {
    return undefined
  }

  return Number(clientId)
}

export const useGetSiteKey = () => {
  const { siteKey } = useParams()

  return siteKey
}

export const useGetCurrentSite = () => {
  const siteKey = useGetSiteKey()

  return {
    siteKey,
    ...useGetSiteByKeyQuery(
      { siteKey: siteKey! },
      { pollingInterval: 3000, skip: !siteKey }
    ),
  }
}
