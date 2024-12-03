import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Suspense } from 'react';
import { createRoot } from 'react-dom/client';
import { RouterProvider } from 'react-router-dom';
import { Loading } from './components/common/loading/index.tsx';
import './index.css';
import router from './routes.tsx';

const queryClient = new QueryClient();

createRoot(document.getElementById('root')!).render(
  <QueryClientProvider client={queryClient}>
    <Suspense fallback={<Loading />}>
      <RouterProvider router={router} />
    </Suspense>
  </QueryClientProvider>
)
