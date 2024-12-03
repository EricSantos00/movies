export function StarIcon({ type, }: { type: 'filled' | 'half' | 'empty' }) {
    const color =
        type === 'filled'
            ? 'text-yellow-500'
            : type === 'half'
                ? 'text-yellow-500'
                : 'text-gray-300';
    return (
        <svg
            className={`w-5 h-5 ${color}`}
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="currentColor"
        >
            <path d="M12 17.27L18.18 21l-1.64-7.03L22 9.24l-7.19-.61L12 2 9.19 8.63 2 9.24l5.46 4.73L5.82 21z" />
            {type === 'half' && <rect x="12" y="0" width="12" height="24" fill="white" />}
        </svg>
    );
};