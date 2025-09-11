import {DrinkLeaderboardEntry} from "@/models/DrinkLeaderboard";
import Table, {TableColumn} from "@/components/layout/Table";
import User from "@/models/User";
import Image from "next/image";
import PhotoIcon from "@/assets/photo.svg";

interface DrinkLeaderboardTableProps {
    title: string;
    leaderboardEntries: DrinkLeaderboardEntry[];
}

export default function DrinkLeaderboardTable({title, leaderboardEntries}: DrinkLeaderboardTableProps) {
    const columns: TableColumn<DrinkLeaderboardEntry>[] = [
        {
            headerName: "User",
            accessor: (item) => (
                <div className="flex gap-2 items-center">
                    {
                        item.user.avatarUrl ?
                            <Image
                                className="size-10 rounded-box"
                                src={item.user.avatarUrl!}
                                alt={item.user.name}
                                width={32}
                                height={32}
                            />
                            :
                            <PhotoIcon className="size-10 rounded-box"/>
                    }

                    <span>{item.user.name}</span>
                </div>
            ),
        },
        {
            headerName: "Count",
            accessor: (item) => (item.drinkCount)
        },
    ];
    return(
        <div className="flex flex-col gap-2">
            <h3 className="text-xl">{title}</h3>
            <Table<DrinkLeaderboardEntry> columns={columns} data={leaderboardEntries} />
        </div>
    )
}