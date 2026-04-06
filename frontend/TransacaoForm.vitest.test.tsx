import { describe, it, expect, vi, beforeEach, afterEach } from "vitest";
import { render, screen, cleanup } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { TransacaoForm } from "./src/TransacaoForm";

const { mockMutateAsync } = vi.hoisted(() => ({
  mockMutateAsync: vi.fn(),
}));

vi.mock("./src/hooks/useCreateTransacao", () => ({
  useCreateTransacao: () => ({
    mutateAsync: mockMutateAsync,
    isPending: false,
  }),
}));

describe("TransacaoForm: bloqueio de receita para menor de idade", () => {
  beforeEach(() => {
    mockMutateAsync.mockReset();
  });

  afterEach(() => {
    cleanup();
  });

  it("mostra aviso quando seleciona pessoa menor de idade", async () => {
    const user = userEvent.setup();
    render(<TransacaoForm />);

    await user.click(screen.getByRole("button", { name: /pessoa menor/i }));

    const aviso = screen.getByText("Menores só podem registrar despesas.");
    expect(aviso).toBeDefined();
    expect(aviso.textContent).toContain("Menores só podem registrar despesas.");
  });

  it("não chama a API quando menor tenta salvar uma receita", async () => {
    const user = userEvent.setup();
    render(<TransacaoForm />);

    await user.selectOptions(screen.getByLabelText("Tipo"), "1");
    await user.click(screen.getByRole("button", { name: /pessoa menor/i }));
    await user.click(screen.getByRole("button", { name: /^salvar$/i }));

    expect(mockMutateAsync).not.toHaveBeenCalled();
  });
});
