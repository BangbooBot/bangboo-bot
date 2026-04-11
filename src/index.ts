import { env } from "#env";
import { bootstrap } from "@constatic/base";
import { GlobalFonts } from "@napi-rs/canvas";

GlobalFonts.loadFontsFromDir("./assets/fonts");
await bootstrap({ meta: import.meta, env });