import PurchaseDrink from "@/components/actions/PurchaseDrink";
import kitchenAPi from "@/services/kitchenApi";
import DrinkLeaderboardTable from "@/components/data/DrinkLeaderboardTable";
import {cookies} from "next/headers";

export default async function DrinksPage() {
    const cookieStore = await cookies();
    const leaderboard = await kitchenAPi.getLeaderboard(cookieStore);
    return (
        <div className="flex flex-col gap-8">
            <h1 className="text-4xl">Drinks</h1>
            <div className="flex flex-col gap-4">
                <PurchaseDrink/>
                <div className="flex flex-wrap gap-4">
                    {
                        leaderboard.isSuccess &&
                            <DrinkLeaderboardTable title="Total" leaderboardEntries={leaderboard.getOrThrow().total} />
                    }
                    {
                        leaderboard.isSuccess &&
                        <DrinkLeaderboardTable title="Weekly" leaderboardEntries={leaderboard.getOrThrow().weekly} />
                    }
                    {
                        leaderboard.isSuccess &&
                        <DrinkLeaderboardTable title="Monthly" leaderboardEntries={leaderboard.getOrThrow().monthly} />
                    }
                    {
                        leaderboard.isSuccess &&
                        <DrinkLeaderboardTable title="Yearly" leaderboardEntries={leaderboard.getOrThrow().yearly} />
                    }
                </div>
            </div>
        </div>
    );
}
