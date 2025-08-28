export const logger = {
  info: (...args: unknown[]) => console.info('[INFO]', ...args),
  warn: (...args: unknown[]) => console.warn('[WARN]', ...args),
  error: (...args: unknown[]) => console.error('[ERROR]', ...args),
}

window.addEventListener('error', (e) =>
  logger.error('GlobalError:', e.error ?? e.message)
)
window.addEventListener('unhandledrejection', (e) =>
  logger.error('UnhandledRejection:', e.reason)
)
