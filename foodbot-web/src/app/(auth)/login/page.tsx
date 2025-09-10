'use client'

import DiscordIcon from "@/assets/discord.svg"
import Link from "next/link";
import authApi from "@/services/authApi";
import {useSearchParams} from "next/navigation";
import {useRouter} from "next/navigation";
import {useEffect, useState} from "react";

export default function LoginPage() {
    const router = useRouter();
    const searchParams = useSearchParams();
    const authCode = searchParams.get("code");
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const auth = async () => {
            if (authCode) {
                const result = await authApi.validateDiscordCode(authCode.toString());
                if (result.isSuccess) {
                    router.push("/");
                } else {
                    setError(result.error);
                }
            }
        }
        auth().then();
    }, [authCode])


    const discordOAuth = process.env.NEXT_PUBLIC_DISCORD_OAUTH_LINK ?? "/login";

    return (
        <div className="flex flex-col gap-4 items-center justify-center h-screen">
            {
                authCode && !error
                ?
                <div className="bg-[#36393e] rounded-field size-fit p-4">
                    <span className="loading loading-spinner loading-2xl text-white"/>
                </div>
                :
                <Link href={discordOAuth} className="btn btn-ghost bg-[#36393e] rounded-field size-fit p-4">
                    <DiscordIcon className="h-8"/>
                </Link>
            }
            {
                (!!error) &&
                <div className="card bg-error text-center text-sm text-error-content p-4">
                    <span>{error}</span>
                    <span>Please try again later</span>
                </div>
            }
        </div>
    );
}
