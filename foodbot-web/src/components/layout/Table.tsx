import {type ReactNode} from "react";

export interface TableColumn<T> {
    headerName: string;
    className?: string;
    accessor: (item: T) => ReactNode;
}

export interface TableProps<T> {
    columns: TableColumn<T>[];
    data: T[];
    actions?: (item: T) => ReactNode;
    className?: string;
}

export default function Table<T>(props: TableProps<T>) {
    const hasActions = props.actions && props.actions.length > 0;

    return (
        <div className="rounded-box bg-base-100 p-4 overflow-x-auto">
            <table className={`${props.className ?? ""} table`}>
                <thead className="">
                <tr>
                    {props.columns.map((column, index) => (
                        <th key={index} className={column.className}>{column.headerName}</th>
                    ))}
                    {hasActions && <th>Actions</th>}
                </tr>
                </thead>
                <tbody className="">
                {props.data.length === 0 ? (
                    <tr>
                        <td colSpan={props.columns.length + (hasActions ? 1 : 0)}>
                            <EmptyContent/>
                        </td>
                    </tr>
                ) : (
                    props.data.map((item, rowIndex) => (
                        <tr key={rowIndex} className="">
                            {props.columns.map((column, columnIndex) => {
                                return <td key={`cell-${columnIndex}`}>{column.accessor(item)}</td>;
                            })}
                            {hasActions && (<td className="flex gap-1">{props.actions?.(item)}</td>)}
                        </tr>
                    ))
                )}
                </tbody>
            </table>
        </div>
    );
}

function EmptyContent() {
    return (
        <div className="flex flex-col items-center justify-center h-full">
            <span className="text-2xl font-bold text-gray-600">No Data</span>
            <span className="text-gray-400">No data available</span>
        </div>
    );
}