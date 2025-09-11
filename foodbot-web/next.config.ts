import type { NextConfig } from "next";

const nextConfig: NextConfig = {
    images: {
        domains: ['cdn.discordapp.com'],
    },
    turbopack: {
        rules: {
            '*.svg': {
                loaders: ['@svgr/webpack'],
                as: '*.js',
            },
        },
    }
};

export default nextConfig;
