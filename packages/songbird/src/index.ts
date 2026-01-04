import { env } from "#env";
import { MubiExtractor } from "#player";
import { Player } from "discord-player";
import { bootstrap } from "@constatic/base";

export const constatic = await bootstrap({ 
    meta: import.meta, env,
    async beforeLoad(client) {
        client.player = new Player(client as never);
        client.player.extractors.register(MubiExtractor, {});
    },
});