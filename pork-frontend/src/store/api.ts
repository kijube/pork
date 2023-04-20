import { emptySplitApi as api } from "./empty-api";
const injectedRtkApi = api.injectEndpoints({
  endpoints: (build) => ({
    getLocalClients: build.query<
      GetLocalClientsApiResponse,
      GetLocalClientsApiArg
    >({
      query: () => ({ url: `/clients/local` }),
    }),
    getLocalClientById: build.query<
      GetLocalClientByIdApiResponse,
      GetLocalClientByIdApiArg
    >({
      query: (queryArg) => ({
        url: `/clients/local/${queryArg.localClientId}`,
      }),
    }),
    getClientLogs: build.query<GetClientLogsApiResponse, GetClientLogsApiArg>({
      query: (queryArg) => ({
        url: `/clients/local/${queryArg.localClientId}/logs`,
        params: { count: queryArg.count, offset: queryArg.offset },
      }),
    }),
    getLocalClientEvents: build.query<
      GetLocalClientEventsApiResponse,
      GetLocalClientEventsApiArg
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
    setClientNickname: build.mutation<
      SetClientNicknameApiResponse,
      SetClientNicknameApiArg
    >({
      query: (queryArg) => ({
        url: `/clients/global/${queryArg.globalClientId}/nickname`,
        method: "PUT",
        body: queryArg.setNicknameRequestDto,
      }),
    }),
    getSites: build.query<GetSitesApiResponse, GetSitesApiArg>({
      query: () => ({ url: `/sites` }),
    }),
    getSiteByKey: build.query<GetSiteByKeyApiResponse, GetSiteByKeyApiArg>({
      query: (queryArg) => ({ url: `/sites/${queryArg.siteKey}` }),
    }),
    getSiteEvals: build.query<GetSiteEvalsApiResponse, GetSiteEvalsApiArg>({
      query: (queryArg) => ({
        url: `/sites/${queryArg.siteKey}/evals`,
        params: { count: queryArg.count, offset: queryArg.offset },
      }),
    }),
    broadcastEval: build.mutation<
      BroadcastEvalApiResponse,
      BroadcastEvalApiArg
    >({
      query: (queryArg) => ({
        url: `/sites/${queryArg.siteKey}/actions/broadcast-eval`,
        method: "POST",
        body: queryArg.broadcastEvalRequestDto,
      }),
    }),
  }),
  overrideExisting: false,
});
export { injectedRtkApi as api };
export type GetLocalClientsApiResponse =
  /** status 200 Success */ LocalClientDto[];
export type GetLocalClientsApiArg = void;
export type GetLocalClientByIdApiResponse =
  /** status 200 Success */ LocalClientDto;
export type GetLocalClientByIdApiArg = {
  localClientId: number;
};
export type GetClientLogsApiResponse = /** status 200 Success */ ClientLogDto[];
export type GetClientLogsApiArg = {
  localClientId: number;
  count: number;
  offset: number;
};
export type GetLocalClientEventsApiResponse = /** status 200 Success */ (
  | InternalMessage
  | InternalEvalResponse
  | InternalFailureResponse
  | InternalHookResponse
  | InternalResponse
  | InternalEvalRequest
  | InternalRequest
)[];
export type GetLocalClientEventsApiArg = {
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
export type RunClientEvalApiResponse =
  /** status 200 Success */ InternalEvalRequest;
export type RunClientEvalApiArg = {
  localClientId: number;
  evalRequestDto: EvalRequestDto;
};
export type SetClientNicknameApiResponse = unknown;
export type SetClientNicknameApiArg = {
  globalClientId: string;
  setNicknameRequestDto: SetNicknameRequestDto;
};
export type GetSitesApiResponse = /** status 200 Success */ SiteDto[];
export type GetSitesApiArg = void;
export type GetSiteByKeyApiResponse = /** status 200 Success */ SiteDto;
export type GetSiteByKeyApiArg = {
  siteKey: string;
};
export type GetSiteEvalsApiResponse =
  /** status 200 Success */ InternalSiteBroadcastMessage[];
export type GetSiteEvalsApiArg = {
  siteKey: string;
  count: number;
  offset: number;
};
export type BroadcastEvalApiResponse = unknown;
export type BroadcastEvalApiArg = {
  siteKey: string;
  broadcastEvalRequestDto: BroadcastEvalRequestDto;
};
export type GlobalClientDto = {
  id: string;
  nickname: string | null;
};
export type SiteNameDto = {
  id: number;
  key: string;
};
export type LocalClientDto = {
  id: number;
  globalClient: GlobalClientDto;
  site: SiteNameDto;
  isOnline: boolean;
  lastSeen: string;
  remoteIp: string | null;
};
export type ClientLogDto = {
  level: string;
  timestamp: string;
  message: string;
};
export type InternalMessage = {
  type: string;
  flowId: string | null;
  timestamp: string;
};
export type InternalResponse = InternalMessage;
export type InternalEvalResponse = InternalResponse & {
  data: string;
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
export type InternalRequest = InternalMessage & {
  sent: boolean;
  sentAt: string | null;
};
export type InternalEvalRequest = InternalRequest & {
  code: string;
  response: InternalEvalResponse;
};
export type EvalRequestDto = {
  code: string;
};
export type SetNicknameRequestDto = {
  nickname: string;
};
export type SiteDto = {
  id: number;
  key: string;
  localClients: LocalClientDto[];
};
export type InternalSiteMessage = {
  type: string;
  flowId: string | null;
  timestamp: string;
};
export type InternalSiteBroadcastMessage = InternalSiteMessage & {
  code: string;
};
export type BroadcastEvalRequestDto = {
  code: string;
};
export const {
  useGetLocalClientsQuery,
  useGetLocalClientByIdQuery,
  useGetClientLogsQuery,
  useGetLocalClientEventsQuery,
  useGetClientConsoleEventsQuery,
  useRunClientEvalMutation,
  useSetClientNicknameMutation,
  useGetSitesQuery,
  useGetSiteByKeyQuery,
  useGetSiteEvalsQuery,
  useBroadcastEvalMutation,
} = injectedRtkApi;
