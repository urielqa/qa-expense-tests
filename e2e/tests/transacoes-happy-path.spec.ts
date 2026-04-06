import { test, expect } from "@playwright/test";
import { logger } from "../helpers/testLogger";
import { linkNaBarraPrincipal } from "../helpers/navLinks";
import { transacoesLocators as loc } from "../resources/transacoesLocators";
import { factories } from "../data/factories";

// Happy path básico: cria uma despesa pra pessoa adulta.
// Precisa de pelo menos uma pessoa e uma categoria cadastradas.
test.describe("Transações — happy paths", () => {
  test("navega para a seção de transações", async ({ page }) => {
    logger.step("home → link transações");
    await page.goto("/");
    await linkNaBarraPrincipal(page, loc.linkTransacoes).click();
    await expect(page.getByRole("heading", { name: loc.headingTransacoes })).toBeVisible();
    logger.success("seção transações visível");
  });

  test("cria transação de despesa para pessoa adulta", async ({ page }) => {
    const descricao = factories.descricaoTransacao();
    logger.step("abrindo formulário de nova transação (despesa)");
    await page.goto("/transacoes");
    await page.getByRole("button", { name: loc.botaoAdicionarTransacao }).click();

    await page.getByLabel(loc.campoDescricao).fill(descricao);
    await page.getByLabel(loc.campoValor).fill("50");
    await page.getByLabel(loc.campoData).fill(new Date().toISOString().slice(0, 10));
    await page.getByLabel(loc.labelTipo).selectOption({ label: loc.opcaoDespesa });

    logger.step("selecionando primeira categoria e pessoa");
    await page.locator(loc.categoriaSelect).fill("");
    await expect(page.getByRole("option").first()).toBeVisible({ timeout: 10_000 });
    await page.getByRole("option").first().click();

    await page.locator(loc.pessoaSelect).fill("");
    await expect(page.getByRole("option").first()).toBeVisible({ timeout: 10_000 });
    await page.getByRole("option").first().click();

    logger.info("submetendo transação — aguardando sucesso na UI");
    await page.getByRole("button", { name: loc.botaoSalvar }).click();

    await expect(
      page.getByText(loc.mensagemSucesso).or(page.getByText(descricao))
    ).toBeVisible({ timeout: 15_000 });
    logger.success("transação de despesa refletida na UI");
  });
});
