export default async function KfcPage() {
    return (
        <div className="flex flex-col gap-8">
            <div
                className="fixed inset-0 bg-cover w-full h-full bg-center bg-no-repeat blur-[4px] -z-10"
                style={{
                    backgroundImage: 'url(https://i.ytimg.com/vi/3k832113v0M/maxresdefault.jpg)'
                }}
            />
            <h1 className="text-4xl">KFC</h1>
        </div>
    );
}
