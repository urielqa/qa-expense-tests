// Sistema de log por nível — dá pra controlar o barulho no terminal sem apagar os logs.
// Uso: LOG_LEVEL=debug npm test (ou ERROR, WARN, INFO, SILENT)

type LogLevel = "SILENT" | "ERROR" | "WARN" | "INFO" | "DEBUG";

const LEVELS: Record<LogLevel, number> = {
  SILENT: 0,
  ERROR: 1,
  WARN: 2,
  INFO: 3,
  DEBUG: 4,
};

function currentLevel(): number {
  const raw = (
    process.env.LOG_LEVEL ||
    process.env.PW_LOG_LEVEL ||
    "INFO"
  ).toUpperCase() as LogLevel;
  return LEVELS[raw] ?? LEVELS.INFO;
}

function should(level: LogLevel): boolean {
  return currentLevel() >= LEVELS[level];
}

export const logger = {
  error: (msg: string, ...args: unknown[]) => {
    if (should("ERROR")) console.log(`❌ ERRO: ${msg}`, ...args);
  },
  warn: (msg: string, ...args: unknown[]) => {
    if (should("WARN")) console.log(`⚠️  AVISO: ${msg}`, ...args);
  },
  info: (msg: string, ...args: unknown[]) => {
    if (should("INFO")) console.log(`ℹ️  ${msg}`, ...args);
  },
  debug: (msg: string, ...args: unknown[]) => {
    if (should("DEBUG")) console.log(`🐛 DEBUG: ${msg}`, ...args);
  },
  success: (msg: string, ...args: unknown[]) => {
    if (should("INFO")) console.log(`✅ ${msg}`, ...args);
  },
  step: (msg: string, ...args: unknown[]) => {
    if (should("DEBUG")) console.log(`👆 STEP: ${msg}`, ...args);
  },
  wait: (msg: string, ...args: unknown[]) => {
    if (should("DEBUG")) console.log(`⏳ WAIT: ${msg}`, ...args);
  },
};
