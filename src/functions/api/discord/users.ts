import { env } from "#env";
import { RouteBases } from "discord.js";

export async function userInfo(token: string): Promise<Response> {
    const url = `${RouteBases.api}/users/@me`;
    return fetch(url, {
        method: "GET",
        headers: {
            "Authorization": `Bearer ${token}`
        }
    });
}