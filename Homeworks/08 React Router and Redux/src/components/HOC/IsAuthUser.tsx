import type { RootState } from '../../store';
import { useSelector } from 'react-redux';
import { Navigate } from 'react-router-dom';

export const IsAuthUser = <P extends object>(
  WrappedComponent: React.ComponentType<P>
): React.FC<P> => {  
  return function IsAuthUser(props: P) {
    const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);

    if (isAuthenticated) {
      return <Navigate to="/" />;
    }
    return <WrappedComponent {...props} />;
  };
};