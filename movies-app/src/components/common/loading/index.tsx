export type LoadingProps = {
    text?: string;
};

export function Loading({ text = 'Loading...' }: LoadingProps) {
    return (
        <div className="flex flex-col items-center justify-center space-y-2 p-4">
            <svg
                className="animate-spin h-8 w-8 text-blue-500"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
            >
                <circle
                    className="opacity-25"
                    cx="12"
                    cy="12"
                    r="10"
                    stroke="currentColor"
                    strokeWidth="4"
                />
                <path
                    className="opacity-75"
                    fill="currentColor"
                    d="M4 12a8 8 0 018-8v8H4z"
                />
            </svg>
            <p className="text-gray-600 text-sm">{text}</p>
        </div>
    );
}