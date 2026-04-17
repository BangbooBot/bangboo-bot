import { bigint, date, pgTable, uniqueIndex, varchar } from "drizzle-orm/pg-core";
import { usersTable } from "./users.js";

export const sessionsTable = pgTable("sessions", {
    id: bigint("id", { mode: "bigint" }).primaryKey().unique(),
    expires_in: date("expires_in", { mode: "date" }).notNull(),
    user_agent: varchar("user_agent").notNull(),
    platform: varchar("platform").notNull(),
    language: varchar("language").notNull(),
    fk_user_id: bigint("fk_user_id", { mode: "bigint" }).notNull().references(() => usersTable.id),
});