import { validateEnv } from "@constatic/base";
import { z } from "zod";
import "./constants.js";

export const env = await validateEnv(z.looseObject({
    BOT_TOKEN: z.string("Discord Bot Token is required").min(1),
    WEBHOOK_LOGS_URL: z.url().optional(),
    GUILD_ID: z.string().optional(),
    SERVER_PORT: z.coerce.number().min(1).optional(),
    CLIENT_ID: z.string(),
    CLIENT_SECRET: z.string(),
    DATABASE_URL: z.url(),
    REDIS_URL: z.url(),
}));