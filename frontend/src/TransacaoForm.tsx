import { useState } from "react";
import { useForm } from "react-hook-form";
import { useCreateTransacao } from "./hooks/useCreateTransacao";

/** Espelha o comportamento documentado do app (menor × receita), sem copiar o código da empresa. */
const TipoTransacao = { Despesa: 0, Receita: 1 } as const;

type PessoaSel = { id: string; nome: string; idade: number };

type FormValues = {
  tipo: number;
};

export function TransacaoForm() {
  const createTransacao = useCreateTransacao();
  const { register, handleSubmit } = useForm<FormValues>({
    defaultValues: { tipo: TipoTransacao.Despesa },
  });

  const [selectedPessoa, setSelectedPessoa] = useState<PessoaSel | null>(null);
  const isMinor = selectedPessoa !== null && selectedPessoa.idade < 18;

  const onSubmit = async (data: FormValues) => {
    if (isMinor && data.tipo === TipoTransacao.Receita) {
      return;
    }
    await createTransacao.mutateAsync({ ...data, pessoaId: selectedPessoa?.id });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <label htmlFor="tipo" className="text-sm font-medium">
          Tipo
        </label>
        <select
          id="tipo"
          {...register("tipo", { valueAsNumber: true })}
          className="select w-full p-2 border rounded"
        >
          <option value={TipoTransacao.Despesa}>Despesa</option>
          <option value={TipoTransacao.Receita} disabled={isMinor}>
            Receita
          </option>
        </select>
      </div>

      <div className="flex gap-2">
        <button
          type="button"
          onClick={() => setSelectedPessoa({ id: "adulto", nome: "Adulto", idade: 30 })}
        >
          Pessoa adulta
        </button>
        <button
          type="button"
          onClick={() => setSelectedPessoa({ id: "menor", nome: "Menor", idade: 15 })}
        >
          Pessoa menor
        </button>
      </div>

      {isMinor && (
        <p className="text-sm text-yellow-600">Menores só podem registrar despesas.</p>
      )}

      <button type="submit">Salvar</button>
    </form>
  );
}
