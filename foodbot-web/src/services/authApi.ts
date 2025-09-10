import baseFetch from "@/lib/baseFetch";

export const authApi = {

    async validateDiscordCode(code: string) {
        return await baseFetch("api/auth/discord/validate", "POST", code);
    },

    async logout() {
        return await baseFetch("api/auth/logout", "POST");
    },
}

export default authApi;