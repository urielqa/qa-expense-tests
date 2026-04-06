# O que encontrei rodando os testes

Fui anotando aqui tudo que não bateu com o esperado enquanto testava.
Ambiente: API local, banco zerado, dados criados pelos próprios testes de integração.
**Não mexi no código da app** — só registrei o que a API devolveu.

Se no teu ambiente o resultado for diferente (versão da API, banco, etc.), atualiza com o status HTTP e um pedaço do body.

---

## 1. Menor com receita

**Regra:** menor não pode ter transação de receita.

**O que fiz:** Criei uma pessoa com data de nascimento de 15 anos atrás, uma categoria de Receita, e tentei POST de uma transação ligando os dois.

**Esperava:** **400** e mensagem que dê para perceber o bloqueio.

**Veio:** **201** — gravou a transação na mesma.

**Porque importa:** entra dado inválido na base; totais e relatório ficam inconsistentes com a regra.

**Gravidade:** alta.

**Onde está o teste:** `MinorIncomeRestrictionTests.Menor_nao_pode_criar_receita` (`Integration/BusinessRules/MinorIncomeRestrictionTests.cs`)

**Nota:** no front de referência o formulário pode mostrar toast e barrar o envio, mas isso **não corrige** a API se ela ainda devolver **201** — o problema continua a ser de contrato/servidor. E2E que cobre o fluxo na UI: `e2e/tests/transacoes-regras.spec.ts`.

---

## 2. Receita usando categoria só de despesa

**Regra:** categoria “só despesa” não devia aceitar transação de receita.

**O que fiz:** categoria com finalidade despesa + POST transação tipo receita com essa categoria.

**Esperava:** **400** com erro de negócio legível.

**Veio:** **500** — parece exceção no servidor em vez de validação tratada.

**Porque importa:** quem consome a API não distingue bem “regra violada” de “coisa partida”; integração fica feia.

**Gravidade:** alta (eu trato como alta porque 500 em fluxo normal é feio; se o time achar média, mudem a etiqueta).

**Onde está o teste:** `CategoryPurposeMismatchTests.Receita_com_categoria_so_despesa_nao_entra` (`Integration/BusinessRules/CategoryPurposeMismatchTests.cs`)

**Contexto:** no domínio a regra levanta `InvalidOperationException`; se a API usar um middleware que trata **qualquer** exceção como **500** genérico, o cliente deixa de ver **400** com corpo de negócio — alinha com o que vimos no item 2.

---

## 3. TransacaoResponse.Valor serializado como double

**Regra implícita:** valores monetários devem preservar precisão decimal.

**O que encontrei:** o record `TransacaoResponse` no contrato de integração
tinha `Valor` tipado como `double`. Valores como `100.50` podem sofrer
imprecisão de ponto flutuante ao desserializar (`100.49999...`).

**Esperava:** `decimal` — mesmo tipo usado em `CreateTransacaoRequest`.

**Impacto:** inconsistência de tipo entre request e response no contrato
de integração; em cenários com casas decimais, a comparação de valores
entre o que foi enviado e o que foi recebido pode falhar silenciosamente.

**Gravidade:** média.

**Correção aplicada:** `TransacaoResponse.Valor` alterado para `decimal`
em `IntegrationTests/Integration/Contracts/ApiContracts.cs`.

---

## Contratos de teste (integração)

- `CreateTransacaoRequest` e `TransacaoResponse` usam **`decimal` em `Valor`** (dinheiro).
  Se a API retornar JSON com número de ponto flutuante e a desserialização quebrar,
  cola um exemplo de JSON aqui e ajusta com nota.

---

## Coisas que, aqui, funcionaram

**Cascata:** criei pessoa + transação, apaguei a pessoa, fiz GET na transação — devolveu **404**. Para mim fecha a regra de não deixar transação órfã *neste ambiente*.

**Teste:** `PersonDeleteCascadeTests.Deletar_pessoa_some_transacoes`

> Se no Postman ou outra máquina o GET ainda der **200**, vale anotar versão da API/BD — pode ser ambiente.

**Categoria “ambos”:** mesma categoria aceitou receita e despesa.

**Teste:** `AmbosCategoryTests.Categoria_ambos_aceita_os_dois_tipos`

---

## Para quem for corrigir

Manda junto: status HTTP, URL do endpoint, e um recorte do JSON (request/response). Ajuda mais do que “deu erro no teste” sem prova.
