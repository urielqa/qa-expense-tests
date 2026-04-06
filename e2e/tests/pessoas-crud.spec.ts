import { test, expect } from "@playwright/test";
import { PessoasPage } from "../pages/PessoasPage";
import { testData } from "../data/testData";
import { factories } from "../data/factories";

test.describe("Pessoas — CRUD", () => {
  test("cria pessoa adulta e aparece na lista", async ({ page }) => {
    const pessoas = new PessoasPage(page);
    await pessoas.abreHome();
    await pessoas.entraEmPessoas();
    await pessoas.confereQueEstaNaAreaPessoas();

    await pessoas.abrirFormularioCriacao();
    const nome = `E2E ${factories.nomePessoa()}`;
    await pessoas.preencherESubmeterPessoa(nome, testData.adulto.dataNascimento);
    await pessoas.confirmarPessoaNaLista(nome);
  });

  test("campos obrigatórios bloqueiam submit vazio", async ({ page }) => {
    const pessoas = new PessoasPage(page);
    await pessoas.abreHome();
    await pessoas.entraEmPessoas();
    await pessoas.abrirFormularioCriacao();
    await pessoas.botaoSalvar.click();
    // Formulário não deve fechar — algum erro de validação deve aparecer
    await expect(pessoas.botaoSalvar).toBeVisible({ timeout: 3_000 });
  });
});
