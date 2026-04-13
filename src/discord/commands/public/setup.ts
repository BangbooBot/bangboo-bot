import { createCommand } from "#base";
import { authPanelMenu } from "#menus";
import { ApplicationCommandType, InteractionReplyOptions } from "discord.js";

createCommand({
    name: "setup",
    description: "setup command",
    type: ApplicationCommandType.ChatInput,
    async run(interaction) {
        const panel = authPanelMenu<InteractionReplyOptions>();
        return await interaction.reply(panel);
    }
});