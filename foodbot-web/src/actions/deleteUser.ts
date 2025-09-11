'use server'

import {revalidatePath} from "next/cache";
import {cookies} from "next/headers";
import userApi from "@/services/userApi";

export default async function deleteUser(formData: FormData) {
    const userId = formData.get("userId");
    if (!userId) return;

    const cookieStore = await cookies();
    const result = await userApi.deleteUser(userId.toString(), cookieStore);

    if (result.isSuccess) {
        revalidatePath('/users')
        return;
    }
}