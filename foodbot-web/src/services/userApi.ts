import baseFetch from "@/lib/baseFetch";
import User from "@/models/User";
import {ReadonlyRequestCookies} from "next/dist/server/web/spec-extension/adapters/request-cookies";

export const authApi = {

    async me(cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch<never, User>("api/user/me", "GET", cookieStore);
    },

    async getList(cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch<never, User[]>("api/user", "GET", cookieStore);
    },

    async addUser(discordId: string, cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch("api/user/add", "POST", cookieStore, {
            discordId: discordId
        });
    },

    async deleteUser(userId: string, cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch(`api/user/${userId}`, "DELETE", cookieStore);
    },

    async deleteMe(cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch("api/user", "DELETE", cookieStore);
    },
}

export default authApi;