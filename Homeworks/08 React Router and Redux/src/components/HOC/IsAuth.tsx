import type { RootState } from '../../store';
import { useSelector } from 'react-redux';
import { Navigate } from 'react-router-dom';

export const IsAuth = <P extends object>(
  WrappedComponent: React.ComponentType<P>

): React.FC<P> => {  
  return function IsAuth(props: P) {
    const isAuthenticated = useSelector((state: RootState) => state.auth.isAuthenticated);
    
    return isAuthenticated ? <WrappedComponent {...props} /> : <Navigate to="/login" replace />;    
  };
};