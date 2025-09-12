import baseFetch from "@/lib/baseFetch";
import {ReadonlyRequestCookies} from "next/dist/server/web/spec-extension/adapters/request-cookies";

export const bankApi = {

    async addMoney(userId: string, amount: number, cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch("api/bank/add", "POST", cookieStore, {
            userId: userId,
            amount: amount
        });
    },
}

export default bankApi;