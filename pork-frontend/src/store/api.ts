import { emptySplitApi as api } from "./empty-api";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    runClientEval: build.mutation<
      RunClientEvalApiResponse,
      RunClientEvalApiArg
    >({
      query: (queryArg) => ({
        url: `/clients/local/${queryArg.localClientId}/actions/eval`,
        method: "POST",
        body: queryArg.evalRequestDto,
      }),
    }),
    getClients: build.query<GetClientsApiResponse, GetClientsApiArg>({
      query: () => ({ url: `/clients` }),
    }),
    setNickname: build.mutation<SetNicknameApiResponse, SetNicknameApiArg>({
      query: (queryArg) => ({
        url: `/clients/global/${queryArg.globalClientId}/nickname`,
        method: "PUT",
        body: queryArg.setNicknameRequestDto,
      }),
    }),
    getClient: build.query<GetClientApiResponse, GetClientApiArg>({
      query: (queryArg) => ({
        url: `/clients/local/${queryArg.localClientId}`,
      }),
    }),
    getClientEvents: build.query<
      GetClientEventsApiResponse,
      GetClientEventsApiArg
    >({
      query: (queryArg) => ({
        url: `/clients/local/${queryArg.localClientId}/events`,
        params: { count: queryArg.count, offset: queryArg.offset },
      }),
    }),
    getClientConsoleEvents: build.query<
      GetClientConsoleEventsApiResponse,
      GetClientConsoleEventsApiArg
    >({
      query: (queryArg) => ({
        url: `/clients/local/${queryArg.localClientId}/events/console`,
        params: { count: queryArg.count, offset: queryArg.offset },
      }),
    }),
    getClientLogs: build.query<GetClientLogsApiResponse, GetClientLogsApiArg>({
      query: (queryArg) => ({
        url: `/clients/local/${queryArg.localClientId}/logs`,
        params: { count: queryArg.count, offset: queryArg.offset },
      }),
    }),
  }),
  overrideExisting: false,
});
export { injectedRtkApi as api };
export type RunClientEvalApiResponse =
  /** status 200 Success */ InternalEvalRequest;
export type RunClientEvalApiArg = {
  localClientId: number;
  evalRequestDto: EvalRequestDto;
};
export type GetClientsApiResponse = /** status 200 Success */ LocalClientDto[];
export type GetClientsApiArg = void;
export type SetNicknameApiResponse = unknown;
export type SetNicknameApiArg = {
  globalClientId: string;
  setNicknameRequestDto: SetNicknameRequestDto;
};
export type GetClientApiResponse = /** status 200 Success */ LocalClientDto;
export type GetClientApiArg = {
  localClientId: number;
};
export type GetClientEventsApiResponse = /** status 200 Success */ (
  | InternalMessage
  | InternalEvalResponse
  | InternalFailureResponse
  | InternalHookResponse
  | InternalResponse
  | InternalEvalRequest
  | InternalRequest
)[];
export type GetClientEventsApiArg = {
  localClientId: number;
  count: number;
  offset: number;
};
export type GetClientConsoleEventsApiResponse = /** status 200 Success */ (
  | InternalMessage
  | InternalEvalResponse
  | InternalFailureResponse
  | InternalHookResponse
  | InternalResponse
  | InternalEvalRequest
  | InternalRequest
)[];
export type GetClientConsoleEventsApiArg = {
  localClientId: number;
  count: number;
  offset: number;
};
export type GetClientLogsApiResponse = /** status 200 Success */ ClientLogDto[];
export type GetClientLogsApiArg = {
  localClientId: number;
  count: number;
  offset: number;
};
export type InternalMessage = {
  type: string;
  flowId: string | null;
  timestamp: string;
};
export type InternalRequest = InternalMessage & {
  sent: boolean;
  sentAt: string | null;
};
export type InternalResponse = InternalMessage;
export type InternalEvalResponse = InternalResponse & {
  data: string;
};
export type InternalEvalRequest = InternalRequest & {
  code: string;
  response: InternalEvalResponse;
};
export type EvalRequestDto = {
  code: string;
};
export type GlobalClientDto = {
  id: string;
  remoteIp: string | null;
  nickname: string | null;
};
export type SiteDto = {
  id: number;
  key: string;
};
export type LocalClientDto = {
  id: number;
  globalClient: GlobalClientDto;
  site: SiteDto;
  isOnline: boolean;
  lastSeen: string;
};
export type SetNicknameRequestDto = {
  nickname: string;
};
export type InternalFailureResponse = InternalResponse & {
  error: string;
};
export type InternalHookResponse = InternalResponse & {
  method: string;
  hookId: string;
  args: string | null;
  result: string | null;
};
export type ClientLogDto = {
  level: string;
  timestamp: string;
  message: string;
};
export const {
  useRunClientEvalMutation,
  useGetClientsQuery,
  useSetNicknameMutation,
  useGetClientQuery,
  useGetClientEventsQuery,
  useGetClientConsoleEventsQuery,
  useGetClientLogsQuery,
} = injectedRtkApi;
