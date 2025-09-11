import userApi from "@/services/userApi";
import ErrorCard from "@/components/layout/ErrorCard";
import Image from "next/image";
import {cookies} from "next/headers";
import User from "@/models/User";
import Table, {TableColumn} from "@/components/layout/Table";
import AddMoneyForm from "@/components/forms/AddMoneyForm";
import DeleteUserForm from "@/components/forms/DeleteUserForm";
import AddUserForm from "@/components/forms/AddUserForm";
import PhotoIcon from "@/assets/photo.svg"

export default async function UsersPage() {
    const cookieStore = await cookies();
    const users = await userApi.getAllList(cookieStore);

    const columns: TableColumn<User>[] = [
        {
            headerName: "User",
            accessor: (item) => (
                <div className="flex gap-2 items-center">
                    {
                        item.avatarUrl ?
                            <Image
                                className="size-10 rounded-box"
                                src={item.avatarUrl!}
                                alt={item.name}
                                width={32}
                                height={32}
                            />
                            :
                            <PhotoIcon className="size-10 rounded-box"/>
                    }

                    <span>{item.name}</span>
                </div>
            ),
        },
        {
            headerName: "Balance",
            accessor: (item) => !!item.id ? <span className={item.money > 0 ? "font-bold text-success" : "font-bold text-error"}>€ {item.money / 100}</span> : "-",
        },
        {
            headerName: "Status",
            accessor: (item) => item.id ? "Joined" : "Not a member",
        },
    ];

    const actions = (item: User) => (
        <div className="flex gap-4 justify-end">
            {!!item.id && <AddMoneyForm userId={item.id ?? ""} />}
            {!!item.id && <DeleteUserForm userId={item.id ?? ""} />}
            {!item.id && <AddUserForm discordId={item.discordId ?? ""} />}
        </div>
    )

    return (
        <div className="flex flex-col gap-8">
            <h1 className="text-4xl">Users</h1>

            {
                users.isSuccess ?
                    <Table<User> columns={columns} data={users.data ?? []} actions={actions}/>
                    :
                    <ErrorCard title="Error" error={users.error ?? undefined}/>
            }
        </div>
    );
}
