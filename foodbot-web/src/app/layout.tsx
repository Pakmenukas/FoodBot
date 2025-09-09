import type {Metadata} from "next";
import {Geist, Geist_Mono} from "next/font/google";
import "./globals.css";
import {ReactNode} from "react";
import Header from "@/components/Header";
import Sidebar from "@/components/Sidebar";
import Footer from "@/components/Footer";

const geistSans = Geist({
    variable: "--font-geist-sans",
    subsets: ["latin"],
});

const geistMono = Geist_Mono({
    variable: "--font-geist-mono",
    subsets: ["latin"],
});

export const metadata: Metadata = {
    title: "FoodFortress",
};

export default function RootLayout({children}: Readonly<{ children: ReactNode }>) {
    return (
        <html lang="en">
        <body className={`${geistSans.variable} ${geistMono.variable} antialiased`}>
        <AppLayout>
            {children}
        </AppLayout>
        </body>
        </html>
    );
}

function AppLayout({children}: Readonly<{ children: ReactNode }>) {
    return (
        <div className="min-h-screen bg-base-300/80 flex flex-col">
            <div
                className="fixed inset-0 bg-cover w-full h-full bg-center bg-no-repeat blur-sm -z-10"
                style={{
                    backgroundImage: 'url(/background.gif)'
                }}
            />

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
    )
}