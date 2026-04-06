// Dados de teste dinâmicos com faker-js. Evita colisão entre execuções e deixa o nome mais real.
import { faker } from "@faker-js/faker/locale/pt_BR";

export const factories = {
  nomePessoa: () => faker.person.fullName(),
  nomeCategoria: (prefixo: string) => `${prefixo} ${faker.word.noun()}`.slice(0, 30),
  descricaoTransacao: () => faker.commerce.productName().slice(0, 50),
};
