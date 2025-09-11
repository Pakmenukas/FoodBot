import type {ReactNode} from "react";
import ColaIcon from "@/assets/cola.svg"
import KfcIcon from "@/assets/kfc.svg"
import UserIcon from "@/assets/user.svg"
import UserSolidIcon from "@/assets/user-solid.svg"
import CreditCardIcon from "@/assets/credit-card.svg"
import CreditCardSolidIcon from "@/assets/credit-card-solid.svg"

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
            {
                icon: <KfcIcon className="size-8"/>,
                iconSelected: <KfcIcon className="size-8"/>,
                name: "KFC",
                path: "/kfc",
            },
        ]
    },
    {
        name: "Banking",
        routes: [
            {
                icon: <CreditCardIcon className="size-8"/>,
                iconSelected: <CreditCardSolidIcon className="size-8"/>,
                name: "Transfers",
                path: "/transfers",
            },
        ]
    },
    {
        name: "Management",
        routes: [
            {
                icon: <UserIcon className="size-8"/>,
                iconSelected: <UserSolidIcon className="size-8"/>,
                name: "Users",
                path: "/users",
            },
        ]
    },
];

export default sidebarRoutes;