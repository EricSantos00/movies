import { Outlet } from "react-router-dom";
import { AppSidebar } from "./appSiderbar";
import { SidebarProvider, SidebarTrigger } from "./ui/sidebar";

export default function Layout() {
    return (
        <div className="min-h-screen w-full flex items-start justify-center">
            <SidebarProvider>
                <AppSidebar />
                <main className="container mx-auto mt-6">
                    <SidebarTrigger />
                    <Outlet />
                </main>
            </SidebarProvider>
        </div>
    )
};