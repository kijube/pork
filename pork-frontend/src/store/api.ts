import { emptySplitApi as api } from "./empty-api";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    getClients: build.query<GetClientsApiResponse, GetClientsApiArg>({
      query: () => ({ url: `/clients` }),
    }),
    getClient: build.query<GetClientApiResponse, GetClientApiArg>({
      query: (queryArg) => ({ url: `/clients/${queryArg.clientId}` }),
    }),
    setNickname: build.mutation<SetNicknameApiResponse, SetNicknameApiArg>({
      query: (queryArg) => ({
        url: `/clients/${queryArg.clientId}/nickname`,
        method: "PUT",
        body: queryArg.setNicknameRequest,
      }),
    }),
    getClientEvents: build.query<
      GetClientEventsApiResponse,
      GetClientEventsApiArg
    >({
      query: (queryArg) => ({
        url: `/clients/${queryArg.clientId}/events`,
        params: { count: queryArg.count, offset: queryArg.offset },
      }),
    }),
    getClientLogs: build.query<GetClientLogsApiResponse, GetClientLogsApiArg>({
      query: (queryArg) => ({
        url: `/clients/${queryArg.clientId}/logs`,
        params: { count: queryArg.count, offset: queryArg.offset },
      }),
    }),
  }),
  overrideExisting: false,
});
export { injectedRtkApi as api };
export type GetClientsApiResponse = /** status 200 Success */ Client[];
export type GetClientsApiArg = void;
export type GetClientApiResponse = /** status 200 Success */ Client;
export type GetClientApiArg = {
  clientId: string;
};
export type SetNicknameApiResponse = unknown;
export type SetNicknameApiArg = {
  clientId: string;
  setNicknameRequest: SetNicknameRequest;
};
export type GetClientEventsApiResponse =
  /** status 200 Success */ SocketEventDateTimeOffset3C3EfAnonymousType0[];
export type GetClientEventsApiArg = {
  clientId: string;
  count: number;
  offset: number;
};
export type GetClientLogsApiResponse =
  /** status 200 Success */ ClientLogEvent[];
export type GetClientLogsApiArg = {
  clientId: string;
  count: number;
  offset: number;
};
export type Client = {
  id: string;
  remoteIp: string | null;
  isOnline: boolean;
  lastSeen: string;
  nickname: string | null;
};
export type SetNicknameRequest = {
  nickname: string;
};
export type SocketEvent = {
  type: string;
  eventId: string | null;
  success: boolean;
};
export type ErrorSocketEvent = SocketEvent & {
  error: string;
};
export type SuccessSocketEvent = SocketEvent;
export type SocketEvalEvent = SuccessSocketEvent & {
  data: string;
};
export type SocketHookEvent = SuccessSocketEvent & {
  method: string;
  hookId: string;
  args: string | null;
  result: string | null;
};
export type SocketEventDateTimeOffset3C3EfAnonymousType0 = {
  event:
    | (
        | SocketEvent
        | ErrorSocketEvent
        | SocketEvalEvent
        | SocketHookEvent
        | SuccessSocketEvent
      )
    | null;
  timestamp: string;
};
export type ClientLogEvent = {
  id: number;
  clientId: string;
  level: string;
  timestamp: string;
  message: string;
};
export const {
  useGetClientsQuery,
  useGetClientQuery,
  useSetNicknameMutation,
  useGetClientEventsQuery,
  useGetClientLogsQuery,
} = injectedRtkApi;
