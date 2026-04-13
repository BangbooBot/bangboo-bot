import { date, pgTable, serial, varchar } from "drizzle-orm/pg-core";

export const users = pgTable("users", {
  id: serial("id").primaryKey(),
  access: varchar("access"),
  type: varchar("type"),
  expires_at: date("expires_at"),
  refresh: varchar("refresh"),
  scope: varchar("scope"),
});