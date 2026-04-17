import type { Client } from "discord.js";
import type { FastifyTypedInstance } from "#types/fastify.js"
import { homeRoute } from "./public/home.js";
import { oauth2LoginRoute } from "./public/oauth2.js";

export function registerRoutes(app: FastifyTypedInstance, client: Client<true>) {
    homeRoute(app, client)
    oauth2LoginRoute(app, client)
}