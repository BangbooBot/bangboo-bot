import { validateEnv } from "@constatic/base";
import { z } from "zod";
import "./constants.js";

export const env = await validateEnv(z.looseObject({
    BOT_TOKEN: z.string("Discord Bot Token is required").min(1),
    WEBHOOK_LOGS_URL: z.url().optional(),
    GUILD_ID: z.string().optional(),
    SERVER_PORT: z.coerce.number().min(1).optional(),
    CLIENT_ID: z.string("Discord Client ID is required").min(1),
    CLIENT_SECRET: z.string("Discord Client Secret is required").min(1),
    DATABASE_URL: z.url("Database URL is required").min(1),
    REDIS_URL: z.url("Redis URL is required").min(1),
}));