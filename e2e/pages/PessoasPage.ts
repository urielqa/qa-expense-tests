import { expect, type Page } from "@playwright/test";
import { pessoasLocators as loc } from "../resources/pessoasLocators";

// Page object de Pessoas: a lógica de interação fica aqui, os seletores ficam em resources/.
// Assim se o app mudar um label, muda em um lugar só.
export class PessoasPage {
  readonly page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  get linkPessoas() {
    return this.page.getByRole("link", { name: loc.linkPessoas });
  }

  get tituloPessoas() {
    return this.page.getByRole("heading", { name: loc.headingPessoas });
  }

  async abreHome() {
    await this.page.goto("/");
  }

  async entraEmPessoas() {
    await this.linkPessoas.click();
  }

  async confereQueEstaNaAreaPessoas() {
    await expect(this.tituloPessoas).toBeVisible();
  }

  get botaoAdicionarPessoa() {
    return this.page.getByRole("button", { name: loc.botaoAdicionarPessoa });
  }
  get campoNome() {
    return this.page.getByLabel(loc.campoNome);
  }
  get campoDataNascimento() {
    return this.page.getByLabel(loc.campoDataNascimento);
  }
  get botaoSalvar() {
    return this.page.getByRole("button", { name: loc.botaoSalvar });
  }
  get botaoCancelar() {
    return this.page.getByRole("button", { name: loc.botaoCancelar });
  }

  async abrirFormularioCriacao() {
    await this.botaoAdicionarPessoa.click();
  }

  async preencherESubmeterPessoa(nome: string, dataNascimento: string) {
    await this.campoNome.fill(nome);
    await this.campoDataNascimento.fill(dataNascimento);
    await this.botaoSalvar.click();
  }

  async confirmarPessoaNaLista(nome: string) {
    await expect(this.page.getByRole("dialog")).toBeHidden({ timeout: 15_000 });
    const tabela = this.page.getByRole("table", { name: "Tabela de dados" });
    await expect(tabela).toBeVisible({ timeout: 15_000 });
    await expect(tabela.getByRole("cell", { name: nome })).toBeVisible({
      timeout: 15_000,
    });
  }
}
