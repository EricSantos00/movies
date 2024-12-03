export type ErrorProps = {
    message?: string;
};

export function Error({ message = 'Something went wrong.' }: ErrorProps) {
    return (
        <div className="flex flex-col items-center justify-center space-y-4 p-4 bg-red-100 rounded-md shadow-md">
            <svg
                className="h-12 w-12 text-red-500"
                xmlns="http://www.w3.org/2000/svg"
                fill="none"
                viewBox="0 0 24 24"
                stroke="currentColor"
                strokeWidth="2"
            >
                <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M18.364 5.636l-12.728 12.728M5.636 5.636l12.728 12.728"
                />
            </svg>
            <p className="text-red-700 text-sm">{message}</p>
        </div>
    );
}