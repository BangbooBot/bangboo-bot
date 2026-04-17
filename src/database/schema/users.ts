import { bigint, date, pgTable, varchar } from "drizzle-orm/pg-core";

export const usersTable = pgTable("users", {
  id: bigint("id", { mode: "bigint" }).primaryKey(),
  access_token: varchar("access_token").notNull(),
  refresh_token: varchar("refresh_token").notNull(),
  token_type: varchar("token_type").notNull(),
  expires_in: date("expires_in", { mode: "date" }).notNull(),
  scope: varchar("scope").notNull(),
  avatar: varchar("avatar"),
});
