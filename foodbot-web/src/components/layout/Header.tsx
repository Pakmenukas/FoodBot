import Image from "next/image";
import Link from "next/link";
import ThemeSwitch from "@/components/actions/ThemeSwitch";
import Logout from "@/components/actions/Logout";
import LoggedInUser from "@/components/data/LoggedInUser";

export default function Header() {
    return (
        <header className="navbar bg-base-100/60 px-4 gap-4">

            <div className="flex-1">
                <label htmlFor="app-drawer" className="btn btn-ghost rounded-field lg:hidden" aria-label="Open Menu">☰</label>
                <Link href="/public" className="btn btn-ghost">
                    <Image
                        className="size-8"
                        src="/icon.png"
                        alt="Food fortress logo"
                        width={16}
                        height={16}
                    />
                    <span className="text-2xl">ᚠᚢᛞ ᚠᚢᚱᛏᚱᛖᛋ</span>
                </Link>
            </div>

            <ThemeSwitch />
            <LoggedInUser />
            <Logout />
        </header>
    )
}