'use client'

import Link from "next/link";
import { usePathname } from 'next/navigation';
import sidebarRoutes from "@/sidebarRoutes";

export default function Sidebar() {
    const pathname = usePathname();
    const baseItemClass = "btn btn-ghost justify-start gap-3 w-full";
    const isActive = (route: string) => pathname === route;
    const linkClass = (route: string ) =>
        `${baseItemClass} ${isActive(route) ? "text-primary" : "text-base-content/80"}`;
    const disabledClass = "btn btn-ghost btn-disabled justify-start gap-3 w-full text-base-content/40";

    return (
        <ul className="menu p-2 w-72 min-h-full space-y-8">
            {sidebarRoutes.map((category, categoryIndex) => (
                <li key={categoryIndex} className="">
                    <span className="text-sm uppercase text-base-content/50 tracking-wide">{category.name}</span>
                    <ul className="space-y-2">
                        {category.routes.map((route, routeIndex) => (
                            <li key={routeIndex} className="">
                                {route.isDisabled ? (
                                    <div className={disabledClass} aria-disabled="true">
                                        <span className="opacity-70">{route.icon}</span>
                                        <span>{route.name}</span>
                                    </div>
                                ) : (
                                    <Link href={route.path} className={linkClass(route.path)}>
                                        {
                                            <>
                                                {isActive(route.path) ?
                                                    <div className="rounded-field w-2 h-6 bg-primary"/> : null}
                                                {isActive(route.path) ? route.iconSelected : route.icon}
                                                <span>{route.name}</span>
                                            </>
                                        }
                                    </Link>
                                )}
                            </li>
                        ))}
                    </ul>
                </li>
            ))}
        </ul>
    )
}