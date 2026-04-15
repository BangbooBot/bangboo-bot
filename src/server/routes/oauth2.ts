import { db, users as usersTable } from "#database";
import { oauth2Authorize, oauth2TokenExchange, userInfo } from "#functions";
import type { FastifyTypedInstance } from "#types/fastify.js";
import { APIUser, Client, RESTPostOAuth2AccessTokenResult } from "discord.js";
import { StatusCodes } from "http-status-codes";
import z from "zod";

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
        async (_, res) => {
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
            const tokenRes = await oauth2TokenExchange(req.query.code);
            if (!tokenRes.ok) {
                const errJson = await tokenRes.json();
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(errJson as any);
            }

            const token = (await tokenRes.json()) as RESTPostOAuth2AccessTokenResult;

            const userRes = await userInfo(token.access_token);
            if (!userRes.ok) {
                const errJson = await userRes.json();
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(errJson as any);
            }

            const user = (await userRes.json()) as APIUser;
            const now = Date.now();
            const expires = new Date(now + token.expires_in * 1000);
            await db.insert(usersTable).values({
                id: BigInt(user.id),
                avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                expires_in: expires,
                access_token: token.access_token,
                refresh_token: token.refresh_token,
                token_type: token.token_type,
                scope: token.scope
            }).onConflictDoUpdate({
                target: usersTable.id,
                set: {
                    avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                    expires_in: expires,
                    access_token: token.access_token,
                    refresh_token: token.refresh_token,
                    token_type: token.token_type,
                    scope: token.scope
                }
            });

            const userData = userSchema.safeParse({
                id: user.id,
                avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                expires_in: expires,
                access_token: token.access_token,
            });

            if (!userData.success) {
                return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(userData.error.issues);
            }

            return res.status(StatusCodes.OK).send(userData.data);
        },
    );
}