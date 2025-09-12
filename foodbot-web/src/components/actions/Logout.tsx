'use client'

import {useRouter} from "next/navigation";
import authApi from "@/services/authApi";

export default function Logout() {
    const router = useRouter();

    const logout = async () => {
        await authApi.logout();
        router.push("/login");
    }

    return <button className="btn btn-ghost rounded-field" onClick={logout}>Logout</button>
}