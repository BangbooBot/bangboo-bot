import { db, sessionsTable, usersTable } from "#database";
import { oauth2Authorize, oauth2RefreshToken, oauth2TokenExchange, userInfo } from "#functions";
import type { FastifyTypedInstance } from "#types/fastify.js";
import type { APIUser, Client, RESTPostOAuth2AccessTokenResult } from "discord.js";
import { eq } from "drizzle-orm";
import { StatusCodes } from "http-status-codes";
import crypto from "node:crypto";
import z from "zod";

const sessionSchema = z.object({
    sessionId: z.string(),
    avatar: z.string().optional(),
    username: z.string(),
});

const errorSchema = z.object({
    error: z.string(),
    message: z.string()
});

export function oauth2LoginRoute(app: FastifyTypedInstance, client: Client<true>) {
    app.post(
        "/oauth2/login",
        {
            schema: {
                summary: "Login user",
                description: "/oauth2/login",
                tags: ["oauth2"],
                headers: z.object({
                    user_agent: z.string(),
                    platform: z.string(),
                    language: z.string(),
                }),
                body: z.object({
                    type: z.enum(["login", "refresh", "logout"]),
                    code: z.string().optional(),
                    session_id: z.bigint().optional(),
                }),
                response: {
                    200: z.object({
                        session_id: z.bigint(),
                        avatar: z.string().optional(),
                        username: z.string(),
                    }),
                    204: z.object(),
                    400: errorSchema,
                    403: errorSchema,
                    500: errorSchema.or(z.array(z.any()))
                }
            },
        },
        async (req, res) => {
            const { body } = req;

            if (body.type === "login") {
                const { code } = body;

                if (!code) {
                    return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to login", message: "Missing code" });
                }

                const tokenRes = await oauth2TokenExchange(code);
                if (!tokenRes.ok) {
                    const errJson = await tokenRes.json();
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to exchange token", message: JSON.stringify(errJson) });
                }
                const token = (await tokenRes.json()) as RESTPostOAuth2AccessTokenResult;
                const userRes = await userInfo(token.access_token);
                if (!userRes.ok) {
                    const errJson = await userRes.json();
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to get user info", message: JSON.stringify(errJson) });
                }
                const user = (await userRes.json()) as APIUser;

                const now = Date.now();
                const expires = new Date(now + (token.expires_in * 1000));
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

                const { user_agent, platform, language } = req.headers;
                const bytes = crypto.randomBytes(32);
                const bigIntFromBuffer = new BigUint64Array(bytes.buffer, bytes.byteOffset, 1)[0];
                await db.insert(sessionsTable).values({
                    id: bigIntFromBuffer,
                    expires_in: expires,
                    user_agent: user_agent,
                    platform: platform,
                    language: language,
                    fk_user_id: BigInt(user.id),
                }).catch((err) => {
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to create session", message: err });
                });

                const newUserData = sessionSchema.safeParse({
                    id: user.id,
                    avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                    expires_in: expires,
                    access_token: token.access_token,
                });
                if (!newUserData.success) {
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send(newUserData.error.issues);
                }
                return res.status(StatusCodes.OK).send({
                    session_id: bigIntFromBuffer,
                    avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                    username: user.username,
                });
            } else if (body.type === "refresh") {
                const { session_id } = body;
                if (!session_id) {
                    return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to refresh token", message: "Missing session id" });
                }

                const [session] = await db.select().from(sessionsTable).where(eq(sessionsTable.id, session_id)).limit(1);
                if (!session) {
                    return res.status(StatusCodes.FORBIDDEN).send({ error: "Failed to refresh token", message: "Invalid session id" });
                }

                if (session.expires_in.getTime() < Date.now()) {
                    return res.status(StatusCodes.FORBIDDEN).send({ error: "Failed to refresh token", message: "Session expired" });
                }

                const [userTable] = await db.select().from(usersTable).where(eq(usersTable.id, session.fk_user_id)).limit(1);
                if (!userTable) {
                    return res.status(StatusCodes.FORBIDDEN).send({ error: "Failed to refresh token", message: "Invalid session id" });
                }

                if (userTable.expires_in.getTime() < Date.now()) {
                    return res.status(StatusCodes.FORBIDDEN).send({ error: "Failed to refresh token", message: "Session expired" });
                }

                const tokenRes = await oauth2RefreshToken(userTable.refresh_token);
                if (!tokenRes.ok) {
                    const errJson = await tokenRes.json();
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to exchange token", message: JSON.stringify(errJson) });
                }
                const token = (await tokenRes.json()) as RESTPostOAuth2AccessTokenResult;
                const userRes = await userInfo(token.access_token);
                if (!userRes.ok) {
                    const errJson = await userRes.json();
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to get user info", message: JSON.stringify(errJson) });
                }
                const user = (await userRes.json()) as APIUser;

                const now = Date.now();
                const expires = new Date(now + (token.expires_in * 1000));

                await db.update(usersTable).set({
                    expires_in: expires,
                    access_token: token.access_token,
                    refresh_token: token.refresh_token,
                    token_type: token.token_type,
                    scope: token.scope
                }).where(eq(usersTable.id, BigInt(user.id))).catch((err) => {
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to update user", message: err });
                });

                await db.update(sessionsTable).set({
                    expires_in: expires,
                }).where(eq(sessionsTable.id, session_id)).catch((err) => {
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to update session", message: err });
                });

                return res.status(StatusCodes.OK).send({
                    session_id: session_id,
                    avatar: user.avatar ? `https://cdn.discordapp.com/avatars/${user.id}/${user.avatar}.png` : undefined,
                    username: user.username,
                });
            }
            else if (body.type === "logout") {
                const { session_id } = body;
                if (!session_id) {
                    return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to logout", message: "Missing session id" });
                }

                const [session] = await db.select().from(sessionsTable).where(eq(sessionsTable.id, session_id)).limit(1);
                if (!session) {
                    return res.status(StatusCodes.FORBIDDEN).send({ error: "Failed to logout", message: "Invalid session id" });
                }

                await db.delete(sessionsTable).where(eq(sessionsTable.id, session_id)).catch((err) => {
                    return res.status(StatusCodes.INTERNAL_SERVER_ERROR).send({ error: "Failed to delete session", message: err });
                });

                return res.status(StatusCodes.NO_CONTENT);
            }

        },
    );
}