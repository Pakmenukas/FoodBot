import baseFetch from "@/lib/baseFetch";
import {ReadonlyRequestCookies} from "next/dist/server/web/spec-extension/adapters/request-cookies";

export const authApi = {

    async validateDiscordCode(code: string, cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch("api/auth/discord/validate", "POST", cookieStore, code);
    },

    async logout(cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch("api/auth/logout", "POST", cookieStore);
    },
}

export default authApi;