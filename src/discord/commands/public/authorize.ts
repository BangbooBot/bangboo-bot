import { createCommand } from "#base";
import { authPanelMenu } from "#menus";
import { ApplicationCommandType, InteractionReplyOptions } from "discord.js";

createCommand({
    name: "authorize",
    description: "authorize command",
    type: ApplicationCommandType.ChatInput,
    async run(interaction) {
        const panel = authPanelMenu<InteractionReplyOptions>();
        return await interaction.reply(panel);
    }
});