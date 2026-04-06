// Locators do formulário de transações centralizados aqui.
// Regra: nenhuma string de seletor espalhada nos specs.
export const transacoesLocators = {
  linkTransacoes: /transa/i,
  headingTransacoes: /transa/i,
  botaoAdicionarTransacao: /adicionar transação/i,
  campoDescricao: "Descrição",
  campoValor: "Valor",
  campoData: "Data",
  labelTipo: "Tipo",
  opcaoReceita: "Receita",
  opcaoDespesa: "Despesa",
  categoriaSelect: "#categoria-select",
  pessoaSelect: "#pessoa-select",
  botaoSalvar: /^salvar$/i,
  /** Texto exato do toast em TransacaoForm (evita colidir com o aviso no formulário). */
  mensagemBloqueioMenor:
    "Menores de 18 anos não podem registrar receitas." as const,
  mensagemSucesso: "Transação salva com sucesso!" as const,
} as const;
