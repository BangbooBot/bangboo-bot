import { Client } from "discord.js";
import type { FastifyTypedInstance } from "#types/fastify.js";
import { StatusCodes } from "http-status-codes";

export function homeRoute(app: FastifyTypedInstance, client: Client<true>){
    app.get("/", (_, res) => {
        return res.status(StatusCodes.OK).send({
            message: `🍃 Online on discord as ${client.user.username}`,
            guilds: client.guilds.cache.size,
            users: client.users.cache.size
        });
    });
}