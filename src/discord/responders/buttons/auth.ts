import { createResponder } from "#base";
import { oauth2Authorize } from "#functions";
import { authInfoMenu } from "#menus";
import { ResponderType } from "@constatic/base";
import { InteractionEditReplyOptions } from "discord.js";

createResponder({
    customId: "/auth/panel/start",
    types: [ResponderType.Button], cache: "cached",
    async run(interaction) {
        await interaction.deferReply();
        const auth = await oauth2Authorize();
        const panel = authInfoMenu<InteractionEditReplyOptions>(auth.url);
        await interaction.editReply(panel);
    },
});