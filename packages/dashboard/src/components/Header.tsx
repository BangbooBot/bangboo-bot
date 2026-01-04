import { Link } from "@tanstack/react-router";
import { useState } from "react";
import { Home, Menu, X } from "lucide-react";
import "@css/navbar.css";
import useMatchMedia from "@/hooks/useMatchMedia";

function DesktopNav() {
    return (
        <div className="w-full max-w-5xl px-8 grid grid-cols-3">
            <Link to="/" className="flex gap-4 w-fit items-center">
                <img
                    src="/images/bangboo/Penguinboo.png"
                    alt="Bangboo"
                    className="h-14 drop-shadow-sm drop-shadow-lime-600"
                />

                <span className="text-white text-2xl font-bold">BANGBOO</span>
            </Link>

            <div className="flex justify-center items-center gap-x-4">
                <Link to="/" className="nav-links">
                    HOME
                </Link>
                <Link to="/" className="nav-links">
                    COMMANDS
                </Link>
                <Link to="/" className="nav-links">
                    STATUS
                </Link>
            </div>

            <div className="flex w-fit ml-auto justify-end items-center">
                <Link to="/" className="nav-login flex items-center px-4 gap-2.5 rounded-md bg-zinc-800 hover:bg-zinc-700/70">
                    <img
                        src="/icons/power.svg"
                        alt="Power icon"
                        className="h-4 fill-lime-300 stroke-lime-300"
                    />
                    <h6 className="text-lime-300">Login</h6>
                </Link>
            </div>
        </div>
    );
}

export default function Header() {
    const [isOpen, setIsOpen] = useState(false);
    const { isDesktop } = useMatchMedia();

    return (
        <>
            <nav className="fixed top-0 w-full flex justify-center py-3 items-center shadow-md shadow-zinc-100/10 dark:shadow-zinc-100/10">
                {DesktopNav()}
            </nav>
        </>
    );
}
