import addUser from "@/actions/addUser";

interface AddUserFormProps {
    discordId: string;
}

export default function AddUserForm({discordId}: AddUserFormProps) {
    return (
        <form action={addUser} className="flex gap-1">
            <input type="hidden" name="discordId" value={discordId ?? ""} />
            <button type="submit" className="btn btn-primary rounded-field">Add</button>
        </form>
    )
}