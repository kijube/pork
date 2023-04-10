import { emptySplitApi as api } from "./empty-api";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    clientEval: build.mutation<ClientEvalApiResponse, ClientEvalApiArg>({
      query: (queryArg) => ({
        url: `/clients/${queryArg.clientId}/actions/eval`,
        method: "POST",
        body: queryArg.evalRequestDto,
      }),
    }),
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
        body: queryArg.setNicknameRequestDto,
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
    getClientConsoleEvents: build.query<
      GetClientConsoleEventsApiResponse,
      GetClientConsoleEventsApiArg
    >({
      query: (queryArg) => ({
        url: `/clients/${queryArg.clientId}/events/console`,
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
export type ClientEvalApiResponse =
  /** status 200 Success */ InternalEvalRequest;
export type ClientEvalApiArg = {
  clientId: string;
  evalRequestDto: EvalRequestDto;
};
export type GetClientsApiResponse = /** status 200 Success */ ClientDto[];
export type GetClientsApiArg = void;
export type GetClientApiResponse = /** status 200 Success */ ClientDto;
export type GetClientApiArg = {
  clientId: string;
};
export type SetNicknameApiResponse = unknown;
export type SetNicknameApiArg = {
  clientId: string;
  setNicknameRequestDto: SetNicknameRequestDto;
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
  clientId: string;
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
  clientId: string;
  count: number;
  offset: number;
};
export type GetClientLogsApiResponse = /** status 200 Success */ ClientLogDto[];
export type GetClientLogsApiArg = {
  clientId: string;
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
export type ClientDto = {
  clientId: string;
  remoteIp: string | null;
  isOnline: boolean;
  lastSeen: string;
  nickname: string | null;
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
  clientId: string;
  level: string;
  timestamp: string;
  message: string;
};
export const {
  useClientEvalMutation,
  useGetClientsQuery,
  useGetClientQuery,
  useSetNicknameMutation,
  useGetClientEventsQuery,
  useGetClientConsoleEventsQuery,
  useGetClientLogsQuery,
} = injectedRtkApi;
