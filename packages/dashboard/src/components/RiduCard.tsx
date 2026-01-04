import { Link } from "@tanstack/react-router";

import { useState } from "react";
import "@css/riducard.css";

export default function Card() {
    return (
        <>
            <div className="card bg-card w-full max-w-[460px] py-4 px-4 rounded-md flex flex-col gap-y-2">
                <section className="bg-card-section w-fit py-1 px-2 flex rounded-full">
                    <h1 className="text-card mx-2 font-sans text-sm font-extrabold">
                        Ridu Newslater
                    </h1>
                </section>
                <section className="bg-card-section py-1 px-2 rounded-md flex">
                    <h1 className="font-[Anton] text-2xl text-card">
                        BANGBOO BOT
                    </h1>
                </section>

                <section className="bg-card-section py-2 px-2 grid min-[515px]:grid-cols-2 grid-cols-1 gap-2 rounded-md">
                    <div className="w-full h-full flex justify-center items-center">
                        <div className="border-card relative w-full max-w-[200px] aspect-square flex justify-center items-center rounded-md border-4">
                            <img
                                src="/images/bangboo/18.png"
                                className="h-full mx-auto"
                            />
                            <div className="bg-card absolute bottom-0 w-full h-16 py-2 flex justify-center items-center">
                                <p className="text-card-section font-extrabold text-2xl">
                                    ENN ENNEN
                                </p>
                            </div>
                        </div>
                    </div>

                    <div className="w-full h-full flex flex-col justify-start items-center gap-y-2">
                        <h1 className="font-[Anton] text-card wrap-break-word text-justify min-[351px]:text-2xl text-xl leading-8">
                            SMALL BODY, BIG HELPPER!
                        </h1>
                        <p className="font-[Anton] text-card wrap-break-word text-justify text-sm">
                            Todo servidor precisa de um coelho autonomo para
                            guia-lo ate mesmo nos cantos mais escuros.
                            Moderação, Comandos divertidos, Notificação,
                            Segurança, etc etc e etc. Tudo em um único sistema.
                        </p>
                    </div>
                </section>

                <section className="card-section-info bg-card-section py-4 px-2 rounded-md flex flex-col justify-between items-center gap-4">
                    <div>
                        <div>
                            <img src="/icons/card-shield.svg" alt="Twitch" />
                            <h5 className="text-card">Moderação</h5>
                        </div>
                        <div>
                            <img src="/icons/card-role.svg" alt="Twitch" />
                            <h5 className="text-card">Cargos</h5>
                        </div>
                        <div>
                            <img src="/icons/card-language.svg" alt="Twitch" />
                            <h5 className="text-card">Idioma</h5>
                        </div>
                        <div>
                            <img src="/icons/card-luck.svg" alt="Twitch" />
                            <h5 className="text-card">Sorteios</h5>
                        </div>
                        <div>
                            <img src="/icons/card-twitch.svg" alt="Twitch" />
                            <h5 className="text-card">Twitch</h5>
                        </div>
                    </div>

                    <div>
                        <div>
                            <div className="bg-card">
                                <p className="text-card-section">400</p>
                            </div>
                            <h5 className="text-card">Servidores</h5>
                        </div>
                        <div>
                            <div className="bg-card">
                                <p className="text-card-section">20</p>
                            </div>
                            <h5 className="text-card">Comandos</h5>
                        </div>
                    </div>
                </section>

                <section className="card-section-links">
                    <div>
                        <a href="">Convide-me</a>
                    </div>
                    <div>
                        <a target="_blank" href="">
                            <img src="/icons/discord.svg" alt="Github" />
                        </a>
                        <a target="_blank" href="">
                            <img src="/icons/github.svg" alt="Github" />
                        </a>
                    </div>
                </section>
            </div>
        </>
    );
}
