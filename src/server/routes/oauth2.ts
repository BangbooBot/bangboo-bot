import { Client } from "discord.js";
import type { FastifyTypedInstance } from "#types/fastify.js";
import { StatusCodes } from "http-status-codes";
import z from "zod";
import { env } from "#env";
import { oauth2Authorize, oauth2Token } from "#functions";

const credentialsSchema = z.object({
    token_type: z.literal("Bearer"),
    access_token: z.string(),
    expires_in: z.number(),
    scope: z.string()
})

export function oauth2AuthorizeRoute(app: FastifyTypedInstance, client: Client<true>) {
    app.get(
        "/oauth2/authorize",
        {
            schema: {
                summary: "Get user authorize",
                description: "/oauth2/authorize?response_type=code&redirect_uri=http://localhost:3001/&scope=email+identify+guilds",
                tags: ["oauth2"],
            },
        },
        async (req, res) => {
            const response = await oauth2Authorize();
            if (!response.ok) {
                const json = await response.json();
                res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(json);
            }
            res.redirect(response.url);
        },
    );
}

export function oauth2TokenRoute(app: FastifyTypedInstance, client: Client<true>) {
    app.get(
        "/oauth2/token",
        {
            schema: {
                summary: "Get user token",
                description: "/oauth2/token?code=<code>",
                tags: ["oauth2"],
                querystring: z.object({
                    code: z.string(),
                }),
                response: {
                    200: credentialsSchema,
                    ...z.object({
                        error: z.string(),
                        error_description: z.string()
                    })
                }
            },
        },
        async (req, res) => {
            const response = await oauth2Token(req.query.code);
            if (!response.ok) {
                const json = await response.json();
                res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(json);
            }
            const json = await response.json();
            const token = credentialsSchema.parse(json);
            res.status(StatusCodes.OK).send(token);
        },
    );
}