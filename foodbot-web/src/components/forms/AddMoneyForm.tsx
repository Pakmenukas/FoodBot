import addMoney from "@/actions/addMoney";

interface AddMoneyFormProps {
    userId: string;
}

export default function AddMoneyForm({userId}: AddMoneyFormProps) {
    return (
        <form action={addMoney} className="flex gap-1">
            <input type="hidden" name="userId" value={userId ?? ""} />
            <input type="text" className="input-sm border-primary border-1 rounded-field w-16 p-1" name="amount" placeholder="Amount" step="0.01" required/>
            <button type="submit" className="btn btn-primary rounded-field">Add</button>
        </form>
    )
}