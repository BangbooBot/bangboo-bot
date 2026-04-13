import { icon } from "#functions";
import { brBuilder, createContainer, createRow, createSeparator } from "@magicyan/discord";
import { ButtonBuilder, ButtonStyle, InteractionEditReplyOptions, type InteractionReplyOptions } from "discord.js";

export function authPanelMenu<R>(): R {
    const customId = (action: string) => `/auth/panel/${action}`;
    const container = createContainer(constants.colors.green,
        brBuilder(
            `# ${icon.icons_stagelocked} Autorização`,
            "Clique no botao abaixo para dar autorização ao bot.",
            "-# **Você pode revogar quando quiser em**",
            "-# **Configurações > Aplicativos autorizados**"
        ),
        createSeparator(false, false),
        createRow(
            new ButtonBuilder({
                customId: customId("start"),
                label: "Iniciar",
                emoji: emojis.static.lock,
                style: ButtonStyle.Success
            })
        )
    );

    return ({
        flags: ["Ephemeral", "IsComponentsV2"],
        components: [container]
    } satisfies InteractionReplyOptions) as R;
}

export function authInfoMenu<R>(body: string): R {
    const customId = (action: string) => `/auth/panel/${action}`;
    const container = createContainer(constants.colors.green,
        brBuilder(
            `# ${icon.icons_stagelocked} Autorização`,
            body
        ),
        createSeparator(false, false),
        createRow(
            new ButtonBuilder({
                customId: customId("start"),
                label: "Iniciar",
                emoji: emojis.static.lock,
                style: ButtonStyle.Success
            })
        )
    );

    return ({
        flags: ["IsComponentsV2"],
        components: [container]
    } satisfies InteractionEditReplyOptions) as R;
}