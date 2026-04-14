import { env } from "#env";
import { RouteBases } from "discord.js";

export async function oauth2Authorize(): Promise<Response> {
    const url = `${RouteBases.api}/oauth2/authorize?client_id=${env.CLIENT_ID}&response_type=code&redirect_uri=+http%3A%2F%2Flocalhost%3A3001%2Foauth2%2Fredirect&scope=identify+guilds+email`;
    return fetch(url, {
        method: "GET",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        }
    });
}

export async function oauth2Token(code: string): Promise<Response> {
    const params = new URLSearchParams({
        client_id: env.CLIENT_ID,
        client_secret: env.CLIENT_SECRET,
        grant_type: "client_credentials",
        scope: "identify connections",
        code
    });
    const res = await fetch(`${RouteBases.api}/oauth2/token`, {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        },
        body: params
    });
    return res;
}