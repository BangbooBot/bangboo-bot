import 'dotenv/config';
import { defineConfig } from 'drizzle-kit';
//import { env } from "./src/env"

export default defineConfig({
  out: './src/db/migrations',
  schema: './src/database/schema/**',
  dialect: 'postgresql',
  dbCredentials: {
    url: process.env.DATABASE_URL,
  },
  casing: "snake_case"
});