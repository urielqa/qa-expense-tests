// Dados de teste via env var — assim dá pra rodar em ambientes diferentes sem mexer no código.
// Copie e2e/.env.example pra e2e/.env e ajuste pro seu setup.

export const testData = {
  adulto: {
    // Nome gerado dinamicamente nos testes via faker — não precisa de env
    dataNascimento: process.env.TEST_ADULTO_NASC || "1990-06-15",
  },
  menor: {
    // Nome com "menor" no texto — procurado pelo LazySelect
    nomeContains: process.env.TEST_MENOR_NOME_CONTAINS || "menor",
    dataNascimento: process.env.TEST_MENOR_NASC || "2015-01-01",
  },
  categoria: {
    nomeReceita: process.env.TEST_CAT_RECEITA_NOME_CONTAINS || "",
  },
} as const;
