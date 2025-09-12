import Result from "@/lib/Result";
import {ReadonlyRequestCookies} from "next/dist/server/web/spec-extension/adapters/request-cookies";

type Method = 'GET' | 'POST' | 'DELETE' | 'PATCH';

export default async function baseFetch<T, R>(
    url: string,
    method: Method,
    cookieStore?: ReadonlyRequestCookies,
    body: T | null = null,
    isAnonymous: boolean = false
): Promise<Result<R>> {

    try {
        const bodyJson = body ? JSON.stringify(body) : null;
        const fullUrl = `${process.env.NEXT_PUBLIC_API_ROOT!}/${url}`
        console.log(method, fullUrl, 'Req body', bodyJson);

        const headers: HeadersInit = {
            'Content-Type': 'application/json',
        };

        if (cookieStore && !isAnonymous) {
            const cookieString = cookieStore.toString();
            if (cookieString) {
                headers['Cookie'] = cookieString;
            }
        }

        const response = await fetch(fullUrl, {
            method: method,
            headers: headers,
            credentials: isAnonymous ? 'omit' : 'include',
            body: bodyJson,
        });
        console.log(method, url, response.status);
        if (!response.ok)
            return Result.failure(response.statusText);

        if (response.status === 204 || response.status === 205) {
            return Result.success(undefined as unknown as R);
        }

        const data: R = await response.json();
        console.debug(url, 'Res body:', data);

        return Result.success(data);
    } catch (e) {
        console.error(url, "Fetch error", e);
        return Result.failure(e?.toString() ?? "Fetch exception occurred");
    }
}