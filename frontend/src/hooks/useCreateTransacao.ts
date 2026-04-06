export type CreateTransacaoPayload = Record<string, unknown>;

export function useCreateTransacao() {
  return {
    mutateAsync: async (_data: CreateTransacaoPayload) => {
      /* substituído em testes via vi.mock */
    },
    isPending: false,
  };
}
