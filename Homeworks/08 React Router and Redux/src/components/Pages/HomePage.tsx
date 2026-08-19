import React from 'react';
import { useSelector } from 'react-redux';
import { type RootState } from '../../store';
import { useDispatch } from 'react-redux';
import { logout } from '../../store/Reducers/authSlice';

const HomePage: React.FC = () => {
  const user = useSelector((state: RootState) => state.auth.user);
  const dispatch = useDispatch();

  return (
    <div>
      <h1>Добро пожаловать, {user}!</h1>
      <button onClick={() => dispatch(logout())}>Выйти</button>
    </div>
  );
};

export default HomePage;