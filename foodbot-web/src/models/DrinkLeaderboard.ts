import User from "@/models/User";

export default interface DrinkLeaderboard {
    total: DrinkLeaderboardEntry[];
    weekly: DrinkLeaderboardEntry[];
    monthly: DrinkLeaderboardEntry[];
    yearly: DrinkLeaderboardEntry[];
}

export type DrinkLeaderboardEntry = {
    user: User;
    drinkCount: number;
}