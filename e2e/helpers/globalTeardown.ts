// Roda depois que todos os testes terminam. Útil pra logar um resumo ou fechar recursos.
import { logger } from "./testLogger";

export default async function globalTeardown() {
  logger.info("Todos os testes E2E finalizados.");
}
