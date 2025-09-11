import type {ReactNode} from "react";
import ColaIcon from "@/assets/cola.svg"
import UserOutlineIcon from "@/assets/user-outline.svg"
import UserSolidIcon from "@/assets/user-solid.svg"

interface SidebarCategory {
    name: string;
    routes: SidebarRoute[];
}

interface SidebarRoute {
    icon: ReactNode;
    iconSelected: ReactNode;
    name: string;
    path: string;
    isDisabled?: boolean;
}

const sidebarRoutes: SidebarCategory[] = [
    {
        name: "Kitchen",
        routes: [
            {
                icon: <ColaIcon className="size-8"/>,
                iconSelected: <ColaIcon className="size-8"/>,
                name: "Drinks",
                path: "/drinks",
            },
        ]
    },
    {
        name: "Management",
        routes: [
            {
                icon: <UserOutlineIcon className="size-8"/>,
                iconSelected: <UserSolidIcon className="size-8"/>,
                name: "Users",
                path: "/users",
            },
        ]
    },
];

export default sidebarRoutes;