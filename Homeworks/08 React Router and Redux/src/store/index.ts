import { configureStore } from '@reduxjs/toolkit';
import authReducer from './Reducers/authSlice';
import usersReducer from './Reducers/usersSlice';

export const store = configureStore({
  reducer: {
    auth: authReducer,
    users: usersReducer
  },
});

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;