import baseFetch from "@/lib/baseFetch";
import {ReadonlyRequestCookies} from "next/dist/server/web/spec-extension/adapters/request-cookies";
import DrinkLeaderboard from "@/models/DrinkLeaderboard";

export const kitchenAPi = {

    async purchaseDrink(cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch<never, {totalPurchasedCount: number}>("api/kitchen/idrink", "POST", cookieStore);
    },

    async getMyCount(cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch<never, {totalPurchasedCount: number}>("api/kitchen/drinks/my", "GET", cookieStore);
    },

    async getLeaderboard(cookieStore?: ReadonlyRequestCookies) {
        return await baseFetch<never, DrinkLeaderboard>("api/kitchen/drinks/leaderboard", "GET", cookieStore);
    },
}

export default kitchenAPi;