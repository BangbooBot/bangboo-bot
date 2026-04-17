import type { Client } from "discord.js";
import type { FastifyInstance } from "fastify";
import { StatusCodes } from "http-status-codes";
import z from "zod";

const authHeaderSchema = z.object({
    user_agent: z.string(),
    platform: z.string(),
    language: z.string(),
});

const authBodySchema = z.object({
    type: z.enum(["login", "refresh", "logout"]),
    code: z.string().optional(),
    session_id: z.bigint().optional(),
});

export function subsystem(app: FastifyInstance, client: Client<true>) {
    app.addHook("onRequest", (req, res, next) => {
        const authRoutes = ["/oauth2/login"];
        const isAuthRoute = authRoutes.some(path => req.url.startsWith(path));

        if (isAuthRoute) {
            const headers = authHeaderSchema.safeParse(req.headers);
            if (!headers.success) {
                return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to login", message: "Missing headers" });
            }

            const body = authBodySchema.safeParse(req.body);
            if (!body.success) {
                return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to login", message: "Missing body" });
            }

            const { data } = body;
            switch (data.type) {
                case "login": {
                    const { code } = data;
                    if (!code) {
                        return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to login", message: "Missing code" });
                    }
                    break;
                }
                case "refresh": {
                    const { session_id } = data;
                    if (!session_id) {
                        return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to refresh token", message: "Missing session id" });
                    }
                    break;
                }
                case "logout": {
                    const { session_id } = data;
                    if (!session_id) {
                        return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to logout", message: "Missing session id" });
                    }
                    break;
                }
            }
        } else if (req.url.startsWith("/private")) {
            const headers = authHeaderSchema.safeParse(req.headers);
            if (!headers.success) {
                return res.status(StatusCodes.BAD_REQUEST).send({ error: "Failed to login", message: "Missing headers" });
            }
        }

        next();
    });
}