'use server'

import bankApi from "@/services/bankApi";
import {revalidatePath} from "next/cache";
import {cookies} from "next/headers";

export default async function addMoney(formData: FormData) {
    const userId = formData.get("userId");
    if (!userId) return;
    const amount = formData.get("amount");
    if (!amount) return;

    const numericAmount = Number(amount);
    if (isNaN(numericAmount)) return;

    const cookieStore = await cookies();
    const result = await bankApi.addMoney(userId.toString(), numericAmount, cookieStore);

    if (result.isSuccess) {
        revalidatePath('/users')
        return;
    }
}