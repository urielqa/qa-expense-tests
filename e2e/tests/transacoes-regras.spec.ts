import { test, expect } from "@playwright/test";
import { logger } from "../helpers/testLogger";
import { linkNaBarraPrincipal } from "../helpers/navLinks";
import { transacoesLocators as loc } from "../resources/transacoesLocators";
import { testData } from "../data/testData";

// Testa a regra de negócio mais crítica no front:
// menor de idade não pode registrar receita.
// Precisa de API + front no ar e uma pessoa com "menor" no nome já cadastrada.
test.describe("Regras de negócio — transações", () => {
  test("bloqueia receita para pessoa menor de idade", async ({ page }) => {
    logger.step("navegando para transações");
    await page.goto("/");
    await linkNaBarraPrincipal(page, loc.linkTransacoes).click();
    await expect(page.getByRole("heading", { name: loc.headingTransacoes })).toBeVisible();

    await page.getByRole("button", { name: loc.botaoAdicionarTransacao }).click();

    logger.step("preenchendo formulário com pessoa menor");
    await page.getByLabel(loc.campoDescricao).fill("E2E bloqueio menor receita");
    await page.getByLabel(loc.campoValor).fill("10");
    await page.getByLabel(loc.campoData).fill(new Date().toISOString().slice(0, 10));

    await page.getByLabel(loc.labelTipo).selectOption({ label: loc.opcaoReceita });

    await page.locator(loc.categoriaSelect).fill("a");
    await expect(page.getByRole("option").first()).toBeVisible({ timeout: 10_000 });
    await page.getByRole("option").first().click();

    const menorPattern = new RegExp(testData.menor.nomeContains, "i");
    await page.locator(loc.pessoaSelect).fill(testData.menor.nomeContains);
    await expect(page.getByRole("option", { name: menorPattern }).first()).toBeVisible({
      timeout: 10_000,
    });
    await page.getByRole("option", { name: menorPattern }).first().click();

    logger.info("clicando em salvar — esperando mensagem de bloqueio");
    await page.getByRole("button", { name: loc.botaoSalvar }).click();

    await expect(page.getByText(loc.mensagemBloqueioMenor)).toBeVisible({
      timeout: 5_000,
    });
    logger.success("regra de menor×receita validada na UI");
  });
});
