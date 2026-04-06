import type { Page } from "@playwright/test";

/**
 * O app renderiza os mesmos itens de menu no header e na sidebar.
 * getByRole('link', …) sozinho dá strict mode violation (2 elementos).
 * Isto fixa no header, que tem aria-label "Main navigation" no React.
 */
export function linkNaBarraPrincipal(page: Page, name: string | RegExp) {
  return page
    .getByRole("navigation", { name: "Main navigation" })
    .getByRole("link", { name });
}
