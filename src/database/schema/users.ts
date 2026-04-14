import { bigint, date, pgTable, varchar } from "drizzle-orm/pg-core";

export const users = pgTable("users", {
  id: bigint("id", { mode: "bigint" }).primaryKey(),
  access: varchar("access").notNull(),
  type: varchar("type").notNull(),
  expires_in: date("expires_in", { mode: "date" }).notNull(),
  scope: varchar("scope").notNull(),
  avatar: varchar("avatar"),
});
