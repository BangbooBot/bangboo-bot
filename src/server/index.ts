import { createEvent } from "#base";
import { env } from "#env";
import {
  serializerCompiler,
  validatorCompiler,
  jsonSchemaTransform,
  type ZodTypeProvider,
} from "fastify-type-provider-zod";
import { fastifySwagger } from "@fastify/swagger";
import { fastifyCors } from "@fastify/cors";
import { fastifyMiddie } from "@fastify/middie";
import ScalarApiReference from "@scalar/fastify-api-reference";
import ck from "chalk";
import fastify from "fastify";
import { registerRoutes } from "./routes/index.js";
import { subsystem } from "./subsystem.js";

const app = fastify().withTypeProvider<ZodTypeProvider>();
app.setValidatorCompiler(validatorCompiler);
app.setSerializerCompiler(serializerCompiler);

createEvent({
  name: "Start Fastify Server",
  event: "clientReady", once: true,
  async run(client) {
    await app.register(
      fastifyCors,
      {
        origin: "*",
        methods: ["GET", "POST", "DELETE"],
        //credentials: true
      }
    );
    await app.register(fastifySwagger, {
      openapi: {
        info: {
          title: "Bangboo API",
          description: "Backend for Bangboo's dashboard",
          version: "0.0.1",
        },
      },
      transform: jsonSchemaTransform,
    });

    await app.register(ScalarApiReference, {
      routePrefix: "/docs",
    });

    await app.register(fastifyMiddie);

    subsystem(app, client);

    registerRoutes(app, client);

    const port = env.SERVER_PORT ?? 3001;

    await app.ready();
    await app.listen({ port, host: "0.0.0.0" })
      .then(() => {
        console.log(ck.green(
          `● ${ck.underline("Fastify")} server listening on port ${port}`
        ));
        console.log(ck.yellow(`📚 Docs avaliable at http://localhost:${port}/docs`));
      })
      .catch(err => {
        console.error(err);
        process.exit(1);
      });
  },
});