import { APIUser, Client } from "discord.js";
import type { FastifyTypedInstance } from "#types/fastify.js";
import { StatusCodes } from "http-status-codes";
import z from "zod";
import { oauth2Authorize, oauth2Token, userInfo } from "#functions";
import { db, users as usersTable } from "#database";

const credentialsSchema = z.object({
    token_type: z.literal("Bearer"),
    access_token: z.string(),
    expires_in: z.number(),
    scope: z.string()
});

const userSchema = z.object({
    id: z.string(),
    avatar: z.string().optional(),
    access_token: z.string(),
    expires_in: z.date(),
});

const errorSchema = z.object({
    error: z.string(),
    error_description: z.string()
});

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
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(json);
            }
            return res.redirect(response.url);
        },
    );
}

export function oauth2RedirectRoute(app: FastifyTypedInstance, client: Client<true>) {
    app.get(
        "/oauth2/redirect",
        {
            schema: {
                summary: "Get user token",
                description: "/oauth2/redirect?code=<code>",
                tags: ["oauth2"],
                querystring: z.object({
                    code: z.string(),
                }),
                response: {
                    200: userSchema,
                    500: errorSchema.or(z.array(z.any()))
                }
            },
        },
        async (req, res) => {
            const tokenRes = await oauth2Token(req.query.code);
            if (!tokenRes.ok) {
                const errJson = await tokenRes.json();
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(errJson as any);
            }

            const token = credentialsSchema.safeParse(await tokenRes.json());
            if (!token.success) {
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(token.error.issues);
            }

            const userRes = await userInfo(token.data.access_token);
            if (!userRes.ok) {
                const errJson = await userRes.json();
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(errJson as any);
            }

            const user = (await userRes.json()) as APIUser;
            const now = Date.now();
            const expires = new Date(now + token.data.expires_in * 1000);
            await db.insert(usersTable).values({
                id: BigInt(user.id),
                avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                expires_in: expires,
                access: token.data.access_token,
                scope: token.data.scope,
                type: token.data.token_type
            }).onConflictDoUpdate({
                target: usersTable.id,
                set: {
                    avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                    expires_in: expires,
                    access: token.data.access_token,
                    type: token.data.token_type,
                    scope: token.data.scope
                }
            });

            const userData = userSchema.safeParse({
                id: user.id,
                avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                expires_in: expires,
                access_token: token.data.access_token,
            });

            if (!userData.success) {
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(userData.error.issues);
            }

            return res.status(StatusCodes.OK).send(userData.data);
        },
    );
}