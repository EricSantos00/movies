import { Error } from '@/components/common/error';
import { useRouteError } from 'react-router-dom';

export default function ErrorPage() {
    const error = useRouteError() as { statusText?: string, message?: string };

    return (
        <Error message={error.statusText || error.message} />
    );
}