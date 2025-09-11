import deleteUser from "@/actions/deleteUser";

interface DeleteUserFormProps {
    userId: string;
}

export default function DeleteUserForm({userId}: DeleteUserFormProps) {
    return (
        <form action={deleteUser} className="flex gap-1">
            <input type="hidden" name="userId" value={userId ?? ""} />
            <button type="submit" className="btn btn-error rounded-field">Delete</button>
        </form>
    )
}