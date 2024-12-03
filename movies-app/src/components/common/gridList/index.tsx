type GridListProps<T> = {
    items: T[];
    renderItem: (item: T) => React.ReactNode;
    title?: string;
};

export function GridList<T>({ items, renderItem, title }: GridListProps<T>) {
    return (
        <div className="p-4 bg-white shadow-lg rounded-lg">
            {title && <h2 className="text-2xl font-bold mb-4">{title}</h2>}
            {items?.length === 0 ? (
                <p className="text-gray-500">No items to display.</p>
            ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-2 gap-6">
                    {items?.map((item, index) => (
                        <div key={index} className="bg-white shadow-lg rounded-lg overflow-hidden">
                            {renderItem(item)}
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}