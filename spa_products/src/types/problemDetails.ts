export type ProblemDetails = {
  type?: string
  title?: string
  status?: number
  detail?: string
  instance?: {
    value?: string
    hasValue?: boolean
  }
  extensions?: {
    errorCode?: string
    correlationId?: string
    traceId?: string
  }
}
