'use client'

import userApi from "@/services/userApi";
import {useEffect, useState} from "react";
import User from "@/models/User";
import Image from "next/image";

export default function LoggedInUser() {
    const [user, setUser] = useState<User | null>(null);
    useEffect(() => {
        userApi.me().then((result) =>{
            setUser(result.data)
        })
    }, []);

    if(!user) return null;

    return(
        <div className="flex gap-2 items-center">
            <Image
                className="rounded-full size-8"
                src={user.avatarUrl}
                alt="User avatar"
                width={32}
                height={32}
            />
            <span className={user.money > 0 ? "font-bold text-success" : "font-bold text-error"}>€ {user.money / 100}</span>
        </div>
    )
}