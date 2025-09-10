import Result from "@/lib/Result";

type Method = 'GET' | 'POST' | 'DELETE' | 'PATCH';

export default async function baseFetch<T, R>(url: string, method: Method, body: T | null = null, isAnonymous: boolean = false): Promise<Result<R>> {

    try {
        const bodyJson = body ? JSON.stringify(body) : null;
        const fullUrl = `${process.env.NEXT_PUBLIC_API_ROOT!}/${url}`
        console.log(fullUrl, 'Req body', bodyJson);

        const response = await fetch(fullUrl, {
            method: method,
            headers: {
                'Content-Type': 'application/json',
            },
            credentials: isAnonymous ? 'omit' : 'include',
            body: bodyJson,
        });
        console.log(url, response.status);
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