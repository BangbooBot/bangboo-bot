import { icon } from "#functions";
import { brBuilder, createContainer, createRow, createSeparator } from "@magicyan/discord";
import { ButtonBuilder, ButtonStyle, InteractionEditReplyOptions, type InteractionReplyOptions } from "discord.js";

export function authPanelMenu<R>(): R {
    //const customId = (action: string) => `/auth/panel/${action}`;
    const url = "http://localhost:3001/oauth2/authorize";
    const container = createContainer(constants.colors.green,
        brBuilder(
            `# ${icon.icons_stagelocked} OAuth2 Autorization`,
            "Click the button below to authorize the bot.",
            "-# **You can revoke it whenever you want at**",
            "-# **Settings > Authorized applications**"
        ),
        createSeparator(false, false),
        createRow(
            new ButtonBuilder({
                label: "Authorize",
                emoji: emojis.static.lock,
                url,
                style: ButtonStyle.Link
            })
        )
    );

    return ({
        flags: ["Ephemeral", "IsComponentsV2"],
        components: [container]
    } satisfies InteractionReplyOptions) as R;
}