import baseFetch from "@/lib/baseFetch";
import User from "@/models/User";

export const authApi = {

    async me() {
        return await baseFetch<never, User>("api/user/me", "GET");
    },
}

export default authApi;