interface ErrorCardProps {
    title: string;
    error?: string;
    description?: string;
}

export default function ErrorCard({title, error, description}: ErrorCardProps) {
    return (
        <div className="card bg-error text-center text-sm text-error-content p-4">
            <span className="text-xl">{title}</span>
            {!!error && <span>{error}</span>}
            {!!description && <span>{description}</span>}
        </div>
    )
}