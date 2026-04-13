import type { Client } from "discord.js";
import type { FastifyTypedInstance } from "#types/fastify.js"
import { homeRoute } from "./home.js";
import { oauth2AuthorizeRoute, oauth2TokenRoute } from "./oauth2.js";

export function registerRoutes(app: FastifyTypedInstance, client: Client<true>) {
    homeRoute(app, client)
    oauth2AuthorizeRoute(app, client)
    oauth2TokenRoute(app, client)
}