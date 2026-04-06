// Reporter customizado: loga qual teste falhou e a primeira linha do erro no terminal.
// Mais limpo do que depender só do HTML report pra saber o que quebrou.
import type { Reporter, TestCase, TestResult } from "@playwright/test/reporter";
import { logger } from "./testLogger";

export default class FailureReporter implements Reporter {
  onTestEnd(test: TestCase, result: TestResult) {
    if (result.status === "failed") {
      logger.error(`FALHOU: ${test.title}`);
      for (const err of result.errors) {
        logger.error(`  → ${err.message?.split("\n")[0] ?? "sem mensagem"}`);
      }
    } else if (result.status === "passed") {
      logger.success(`PASSOU: ${test.title}`);
    }
  }

  onEnd() {
    logger.info("Suite E2E encerrada.");
  }
}
