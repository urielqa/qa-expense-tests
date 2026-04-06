import * as path from "node:path";
import { config as loadEnv } from "dotenv";
import { defineConfig } from "@playwright/test";

loadEnv({ path: path.resolve(__dirname, ".env") });

export default defineConfig({
  testDir: "./tests",
  timeout: 30_000,
  retries: 1,
  globalTeardown: path.join(__dirname, "helpers", "globalTeardown.ts"),
  reporter: [
    ["html", { open: "never" }],
    [path.join(__dirname, "helpers", "failureReporter.ts")],
  ],
  use: {
    baseURL: process.env.PLAYWRIGHT_BASE_URL || "http://localhost:5173",
    screenshot: "only-on-failure",
    video: "retain-on-failure",
  },
});
