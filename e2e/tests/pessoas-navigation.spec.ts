import { test } from "@playwright/test";
import { PessoasPage } from "../pages/PessoasPage";

// Smoke: home → Pessoas. Locators finos ficam na page, aqui só o fluxo.
test("abre Pessoas a partir da home", async ({ page }) => {
  const pessoas = new PessoasPage(page);
  await pessoas.abreHome();
  await pessoas.entraEmPessoas();
  await pessoas.confereQueEstaNaAreaPessoas();
});
