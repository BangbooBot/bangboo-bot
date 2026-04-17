import { env } from "#env";
import { RESTPostOAuth2AccessTokenResult, RouteBases } from "discord.js";

//type TokenExchangeResult = RESTPostOAuth2AccessTokenResult

export async function oauth2Authorize(): Promise<Response> {
    const params = new URLSearchParams({
        client_id: env.CLIENT_ID,
        response_type: "code",
        redirect_uri: "http://localhost:3000/",
        scope: "identify guilds email"
    });
    const url = `${RouteBases.api}/oauth2/authorize?${params.toString()}`;
    return fetch(url, {
        method: "GET",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        }
    });
}

export async function oauth2TokenExchange(code: string): Promise<Response> {
    const params = new URLSearchParams({
        client_id: env.CLIENT_ID,
        client_secret: env.CLIENT_SECRET,
        grant_type: "authorization_code",
        code,
        redirect_uri: "http://localhost:3000/"
    });
    const res = await fetch(`${RouteBases.api}/oauth2/token`, {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        },
        body: params,
    });
    return res;
}

export async function oauth2RefreshToken(refresh_token: string): Promise<Response> {
    const params = new URLSearchParams({
        client_id: env.CLIENT_ID,
        client_secret: env.CLIENT_SECRET,
        grant_type: "refresh_token",
        refresh_token,
        redirect_uri: "http://localhost:3000/"
    });
    const res = await fetch(`${RouteBases.api}/oauth2/token`, {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded"
        },
        body: params,
    });
    return res;
}