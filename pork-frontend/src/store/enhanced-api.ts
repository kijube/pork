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
        getClientConsoleEvents: {
            providesTags: (result, error, arg) =>
                result ? providesList(result as any, "events") : [],
        },
        clientEval: {
            invalidatesTags: (result, error, arg) =>
                !error
                    ? [
                          { type: "clients", id: arg.clientId },
                          { type: "events", id: "LIST" },
                      ]
                    : [],
        },
    },
})

// get type of array element
export type ClientConsoleEvent = GetClientEventsApiResponse[number]
