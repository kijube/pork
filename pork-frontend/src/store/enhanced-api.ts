import { api, SocketEventDateTimeOffset3C3EfAnonymousType0 } from "./api"

function providesList<R extends { id: string | number }[], T extends string>(
  resultsWithIds: R | undefined,
  tagType: T
) {
  return resultsWithIds
    ? [
        { type: tagType, id: "LIST" },
        ...resultsWithIds.map(({ id }) => ({ type: tagType, id })),
      ]
    : [{ type: tagType, id: "LIST" }]
}

export const enhancedApi = api.enhanceEndpoints({
  addTagTypes: ["clients"],
  endpoints: {
    getClient: {
      providesTags: (result, error, arg) =>
        result ? [{ type: "clients", id: arg.clientId }] : [],
    },
    getClients: {
      providesTags: (result, error, arg) =>
        result ? providesList(result as any, "clients") : [],
    },
    setNickname: {
      invalidatesTags: (result, error, arg) =>
        !error ? [{ type: "clients", id: arg.clientId }] : [],
    },
  },
})

export type AnySocketEvent = SocketEventDateTimeOffset3C3EfAnonymousType0
