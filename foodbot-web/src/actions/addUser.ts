'use server'

import {revalidatePath} from "next/cache";
import {cookies} from "next/headers";
import userApi from "@/services/userApi";

export default async function addUser(formData: FormData) {
    const discordId = formData.get("discordId");
    if (!discordId) return;

    const cookieStore = await cookies();
    const result = await userApi.addUser(discordId.toString(), cookieStore);

    if (result.isSuccess) {
        revalidatePath('/users')
        return;
    }
}