import { Client } from "discord.js";
import type { FastifyTypedInstance } from "#types/fastify.js";
import { StatusCodes } from "http-status-codes";
import z from "zod";

export function homeRoute(app: FastifyTypedInstance, client: Client<true>) {
    app.get("/public",
        {
            schema: {
                summary: "Get bot status",
                description: "Get bot status",
                tags: ["home"],
                response: {
                    200: z.object({
                        message: z.string(),
                        guilds: z.number(),
                        users: z.number(),
                        commands: z.number()
                    })
                }
            },
        },
        async (_, res) => {
            return res.status(StatusCodes.OK).send({
                message: `🍃 Online on discord as ${client.user.username}`,
                guilds: client.guilds.cache.size,
                users: client.users.cache.size,
                commands: client.application.commands.cache.size
            });
        });
}