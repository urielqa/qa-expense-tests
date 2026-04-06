import { test, expect } from "@playwright/test";
import { linkNaBarraPrincipal } from "../helpers/navLinks";
import { categoriasLocators as loc } from "../resources/categoriasLocators";
import { factories } from "../data/factories";

test.describe("Categorias — CRUD", () => {
  test("navega para a seção de categorias", async ({ page }) => {
    await page.goto("/");
    await linkNaBarraPrincipal(page, loc.linkCategorias).click();
    await expect(page.getByRole("heading", { name: loc.headingCategorias })).toBeVisible();
  });

  test("cria categoria de despesa e aparece na lista", async ({ page }) => {
    await page.goto("/");
    await linkNaBarraPrincipal(page, loc.linkCategorias).click();
    await page.getByRole("button", { name: loc.botaoAdicionarCategoria }).click();

    const descricao = factories.nomeCategoria("E2E Desp");
    await page.getByLabel(loc.campoDescricao).fill(descricao);
    await page.getByRole(loc.selectFinalidadeRole).selectOption({ label: loc.opcaoDespesa });
    await page.getByRole("button", { name: loc.botaoSalvar }).click();

    await expect(page.getByText(descricao)).toBeVisible({ timeout: 8_000 });
  });
});
