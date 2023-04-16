import {
  api,
  GetClientConsoleEventsApiResponse,
  GetClientEventsApiResponse,
} from "./api"

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
  addTagTypes: ["clients", "events"],
  endpoints: {
    getClient: {
      providesTags: (result, error, arg) =>
        result ? [{ type: "clients", id: result.globalClient.id }] : [],
    },
    getClients: {
      providesTags: (result, error, arg) =>
        result
          ? [
              { type: "clients", id: "LIST" },
              ...result.map((c) => ({
                type: "clients" as const,
                id: c.globalClient.id,
              })),
            ]
          : [],
    },
    setNickname: {
      invalidatesTags: (result, error, arg) =>
        !error ? [{ type: "clients", id: arg.globalClientId }] : [],
    },
    getClientConsoleEvents: {
      providesTags: (result, error, arg) =>
        result ? providesList(result as any, "events") : [],
    },
    runClientEval: {
      invalidatesTags: (result, error, arg) =>
        result ? [{ type: "events", id: "LIST" }] : [],
    },
  },
})

// get type of array element
export type ClientConsoleEvent = GetClientEventsApiResponse[number]
