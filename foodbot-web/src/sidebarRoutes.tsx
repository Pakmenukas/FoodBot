import type {ReactNode} from "react";
import ColaIcon from "@/assets/cola.svg"

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
    }
];

export default sidebarRoutes;