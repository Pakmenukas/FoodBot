import {ReactNode} from "react";
import Header from "@/components/layout/Header";
import Sidebar from "@/components/layout/Sidebar";
import Footer from "@/components/layout/Footer";

export default function DashboardLayout({children}: Readonly<{ children: ReactNode }>) {
    return (

        <div className="min-h-screen bg-base-300/80 flex flex-col">
            <Header />

            <div className="flex flex-1">
                {/* Sidebar - hidden on mobile, visible on lg+ */}
                <aside className="hidden lg:flex w-72 bg-base-300/60">
                    <Sidebar />
                </aside>

                {/* Mobile sidebar overlay */}
                <div className="lg:hidden">
                    <div className="drawer">
                        <input id="app-drawer" type="checkbox" className="drawer-toggle" />
                        <div className="drawer-side z-30">
                            <label htmlFor="app-drawer" className="drawer-overlay"></label>
                            <aside className="w-72 min-h-full bg-base-300">
                                <Sidebar />
                            </aside>
                        </div>
                    </div>
                </div>

                {/* Main content area */}
                <div className="flex-1 flex flex-col min-w-0 overflow-y-auto">
                    <main className="flex-1 m-2 lg:m-4 p-4 lg:p-6 bg-base-200/50 rounded-box">
                        {children}
                    </main>

                    <Footer />
                </div>
            </div>
        </div>
    );
}