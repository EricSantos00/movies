import { Outlet } from "react-router-dom";

export default function Layout() {
    return (
        <div className="min-h-screen w-full flex items-start justify-center">
            <main className="container mx-auto mt-6">
                <Outlet />
            </main>
        </div>
    )
};