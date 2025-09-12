import type { NextRequest } from "next/server";
import { NextResponse } from "next/server";

export default async function middleware(request: NextRequest) {
    const response = NextResponse.next();
    const pathname = request.nextUrl.pathname;
    const loggedIn = request.cookies.get("Cookies");

    const authUrls = new Set(["/login"]);

    if (!loggedIn && !authUrls.has(pathname)) {
        return NextResponse.redirect(new URL("/login", request.url));
    }

    if (loggedIn && authUrls.has(pathname)) {
        return NextResponse.redirect(new URL("/", request.url));
    }

    return response;
}

// Don't run middleware for these files
export const config = {
    matcher: [
        /*
         * Match all request paths except for the ones starting with:
         * - api (API routes)
         * - _next/static (static files)
         * - _next/image (image optimization files)
         * - favicon.ico (favicon file)
         * - background.gif (background image file)
         */
        "/((?!api|_next/static|_next/image|favicon.ico|background).*)",
    ],
};