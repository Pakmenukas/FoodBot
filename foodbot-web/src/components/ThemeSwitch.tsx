'use client'

import {useEffect, useState} from "react";
import SunIcon from "@/assets/sun.svg"
import MoonIcon from "@/assets/moon.svg"

export default function ThemeSwitch() {

    const darkModeName = "dark";

    const [isDark, setIsDark] = useState(false);
    const [mounted, setMounted] = useState(false);

    useEffect(() => {
        setMounted(true);
        const savedTheme = localStorage.getItem('isdark') === "yes";
        setIsDark(savedTheme);
    }, []);

    useEffect(() => {
        if (mounted) {
            localStorage.setItem('isdark', isDark ? "yes" : "no");
        }
    }, [isDark, mounted]);

    if (!mounted) {
        return (
            <label className="swap swap-rotate size-4">
                <input type="checkbox" className="theme-controller" value={darkModeName} checked={false} readOnly
                       aria-label="Toggle dark mode"/>
                <SunIcon className="swap-off size-6"/>
                <MoonIcon className="swap-on size-6"/>
            </label>
        );
    }

    return (
        <label className="swap swap-rotate size-4">
            <input type="checkbox" className="theme-controller" value={darkModeName} checked={isDark}
                   onChange={() => setIsDark(!isDark)} aria-label="Toggle dark mode"/>
            <SunIcon className="swap-off size-6"/>
            <MoonIcon className="swap-on size-6"/>
        </label>
    )
}