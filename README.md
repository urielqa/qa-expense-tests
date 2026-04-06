# Suíte de testes — controle de gastos

Repo **só com testes** para a avaliação. A aplicação (API/React) vem no pacote que a empresa passa — **você** sobe API e front no projeto deles; aqui dentro não tem código da app.

No começo usei **Postman** à mão para bater nos endpoints e ver resposta/status antes de fechar a automação no xUnit.

---

## Pirâmide de testes (visão rápida)

```
        ┌─────────┐
        │   E2E   │  Playwright + TS — Pessoas + regras (ex.: menor × receita na UI)
       ─┴─────────┴─
      │ Integration │  HTTP real (xUnit + HttpClient) — contrato, status, efeitos na API
     ─┴─────────────┴─
    │      Unit       │  Regras no **Domain** (xUnit) — menor/receita, tipo × categoria
   ─┴─────────────────┴─
```

| Camada | O quê | Onde no repo |
|--------|--------|----------------|
| **Unit** | Validação **direta** das regras no domínio (`Transacao`, `Categoria.PermiteTipo`) | `backend/unit/MinhasFinancas.UnitTests/` |
| **Integration** | Comportamento **entre camadas** via API (o que o cliente HTTP vê) | `IntegrationTests/` |
| **E2E** | Fluxo **como usuário** no browser | `e2e/` |

---

## Por que testei o que testei

Não fui atrás de cobertura total — isso não faria sentido sem conhecer o backlog real.
Foquei nas três regras que, se quebrarem, geram **dados inconsistentes na base e nos totais**:

- **Menor + receita** — se isso passar na API, entra dado errado na base sem aviso.
- **Tipo × finalidade da categoria** — mistura errada e o relatório financeiro mente.
- **Cascata ao deletar pessoa** — transação órfã é lixo que nunca some sozinho.

As três têm cobertura em dois níveis: unitário (regra isolada no domain) e integração (o que o HTTP realmente devolve).

---

## Decisão técnica: reflexão nos testes unitários

Inicialmente, os testes não conseguiam acionar as validações do domínio.
O problema é que os setters de `Categoria` e `Pessoa` em `Transacao` são `internal` — de fora do assembly não tem como atribuir diretamente.

A solução que encontrei foi usar reflexão só nos testes (`TransacaoNavigation`): chama o setter interno e, se vier `TargetInvocationException`, relança a exceção **de dentro** pra assert ficar limpo.

Não alterei o código da aplicação. O “hack” fica **isolado** nesse helper — trade-off consciente entre não tocar no repo da empresa e ainda exercitar as mesmas regras que estão nas entidades.

---

## O que você precisa ter

- SDK .NET (**net10.0** nos testes de integração; unit referencia o **Domain** em `net9.0` mas o projeto de testes compila em net10).
- **API** no ar para integração (eu usei `http://localhost:5000`).
- **Swagger** por perto pra conferir o JSON.
- Para **unit** no Domain: o `.csproj` do `MinhasFinancas.Domain` acessível no disco (veja abaixo).
- Para E2E: **front** no ar (ex. `http://localhost:5173`).

A inicialização da API e do front depende do pacote fornecido pela empresa, por isso não repliquei as instruções aqui.

---

## Unit (.NET / xUnit + Domain)

Referência ao projeto **`MinhasFinancas.Domain`** (sem copiar código da app para cá):

- Caminho **padrão** no `.csproj`: pasta do exame ao lado da árvore que contém `qa-expense-tests` (`ExameDesenvolvedorDeTestes\...\MinhasFinancas.Domain.csproj`), **ou**
- Copie `backend/MinhasFinancas.Repo.props.example` → `backend/MinhasFinancas.Repo.props` e ajuste o caminho (**gitignore**), **ou**
- Passe na linha de comando: `/p:MinhasFinancasDomainCsproj=C:\...\MinhasFinancas.Domain.csproj`

```bash
cd backend/unit/MinhasFinancas.UnitTests
dotnet test /p:MinhasFinancasDomainCsproj="C:\SEU_CAMINHO\MinhasFinancas.Domain.csproj"
```

---

## Integração (.NET / xUnit)

```bash
cd IntegrationTests
dotnet test
```

**URL da API:** `IntegrationTests/appsettings.Integration.json` ou `EXPENSE_API_BASE_URL`.

---

## E2E (Playwright)

### Pré-condições para E2E

Os testes assumem **API + front no ar** e **dados mínimos** já na base (o spec `transacoes-regras` pesquisa pessoa/categoria pelos campos da UI):

- **Pessoa menor:** pelo menos uma pessoa com **“menor” no nome** e data de nascimento que a app trate como **menor de 18 anos** (o teste filtra pelo LazySelect de pessoas).
- **Categoria de receita:** pelo menos uma categoria com **finalidade Receita** (e descrição que apareça na lista ao pesquisar — o fluxo escolhe receita como tipo e precisa de categoria compatível).

Você pode criar isso **à mão na UI**, pelo **Postman/Swagger** ou com um script que bata na API **antes** de `npm test`. Sem esses dados, o fluxo de regras tende a falhar por “não achou opção”, não por bug da app.

```bash
cd e2e
npm install
npx playwright install chromium   # uma vez
npm test
```

**Config e pastas:** `e2e/playwright.config.ts` (timeout 30s, 1 retry, relatório HTML, `failureReporter`, `globalTeardown`, carrega `e2e/.env` via `dotenv`). Use `PLAYWRIGHT_BASE_URL` se o front não for `http://localhost:5173`.

- Locators: `e2e/resources/`
- Dados / faker: `e2e/data/`
- Logs: `e2e/helpers/testLogger.ts`
- Page object (Pessoas): `e2e/pages/PessoasPage.ts`
- Regra **menor × receita** na UI: `e2e/tests/transacoes-regras.spec.ts`

### Variáveis de ambiente

| Variável | Padrão | Uso |
|----------|--------|-----|
| `PLAYWRIGHT_BASE_URL` | `http://localhost:5173` | URL do front para E2E |
| `EXPENSE_API_BASE_URL` | `http://localhost:5000` | URL da API para testes de integração |
| `LOG_LEVEL` | `INFO` | Verbosidade dos logs E2E (`SILENT` / `ERROR` / `WARN` / `INFO` / `DEBUG`) |
| `TEST_MENOR_NOME_CONTAINS` | `menor` | Texto buscado no LazySelect para encontrar pessoa menor |
| `TEST_ADULTO_NASC` | `1990-06-15` | Data de nascimento do adulto criado nos testes E2E |
| `TEST_MENOR_NASC` | `2015-01-01` | Data de nascimento do menor (referência; o fluxo E2E usa sobretudo o nome) |
| `DOMAIN_CSPROJ_PATH` | _(vazio)_ | Caminho do `MinhasFinancas.Domain.csproj` (job `unit` no GitHub Actions) |

Copie `e2e/.env.example` → `e2e/.env` e ajuste para o seu ambiente.

---

## Vitest (componentes React)

Pasta **`frontend/`** isolada: componente mínimo espelhando a regra **menor × receita** (sem copiar o código da app), com `useCreateTransacao` mockável nos testes — atende ao pedido de **Vitest + React/TypeScript** sem incluir o bundle da aplicação da empresa, atendendo ao requisito do edital.

```bash
cd frontend
npm install
npm test
```

Arquivos: `frontend/TransacaoForm.vitest.test.tsx`, `frontend/src/TransacaoForm.tsx`, `vite.config.ts` + `tsconfig.json`.

---

## CI (GitHub Actions)

O workflow em `.github/workflows/tests.yml` dispara automaticamente em push e pull request para `main`.

Os jobs usam **`continue-on-error: true`** porque o **Domain**, a **API** e o **front** não vêm neste repositório; assim o workflow não bloqueia o push, mas deixa documentado **como** rodar quando esses serviços existirem (por exemplo em outro pipeline com a app disponível).

| Job | O que faz | Comportamento no CI |
|-----|-----------|---------------------|
| **unit** | `dotnet test` com `MinhasFinancasDomainCsproj` = env `DOMAIN_CSPROJ_PATH` do workflow (vazio por padrão) | `continue-on-error: true` no passo. Sem apontar o `.csproj` do Domain no CI, o passo falha; localmente: `/p:MinhasFinancasDomainCsproj=...` |
| **integration** | `dotnet test` nos testes de integração | Falha esperada: API não está no ar no CI. `continue-on-error: true` |
| **E2E** | `npm test` com Playwright | Falha esperada: front não está no ar no CI. `continue-on-error: true` |

A estrutura do workflow está pronta para rodar em ambiente com a aplicação disponível (ex.: pipeline do time com a API/front como serviço).

---

## Como o repo está organizado

```
backend/unit/     → testes unitários (xUnit, Domain direto)
IntegrationTests/ → testes de integração via HTTP
frontend/         → Vitest + RTL (regra menor×receita isolada)
e2e/              → Playwright (fluxos completos no browser)
docs/             → BUGS.md com o que encontrei
```

O `e2e/` tem subpastas: `resources/` pra locators centralizados (um lugar só pra mudar), `data/` pra dados de teste e faker, `helpers/` pra logger e reporter. Specs em `e2e/tests/`, page objects em `e2e/pages/`. CI em `.github/workflows/tests.yml`.

---

## Bugs

**`docs/BUGS.md`** — notas do que vi na API (status, body, teste que reproduz).

---

## Notas rápidas

- Não alterei código da aplicação; só testes e esta doc.
- Em caso de falha: verifique se a API e o front estão no ar, se as URLs estão corretas e se o caminho do **Domain** foi configurado nos testes unitários.
