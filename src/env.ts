import { validateEnv } from "@constatic/base";
import { z } from "zod";
import "./constants.js";

export const env = await validateEnv(z.looseObject({
    BOT_TOKEN: z.string("Discord Bot Token is required").min(1),
    WEBHOOK_LOGS_URL: z.url().optional(),
    GUILD_ID: z.string().optional(),
    DISCLOUD_TOKEN: z.string("Discloud token is required").min(1),
    GEMINI_API_KEY: z.string("Gemini API key is required").min(1)
}));