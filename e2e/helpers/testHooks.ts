// Tipos auxiliares para o reporter de falhas.
import { logger } from "./testLogger";

export async function onTestFailed(testInfo: {
  title: string;
  errors: { message?: string }[];
}) {
  logger.error(`Teste falhou: ${testInfo.title}`);
  for (const err of testInfo.errors) {
    logger.error(`Detalhe: ${err.message ?? "sem mensagem"}`);
  }
}
