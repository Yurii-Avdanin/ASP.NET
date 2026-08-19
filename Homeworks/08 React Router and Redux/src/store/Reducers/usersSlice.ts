import { createSlice, type PayloadAction } from '@reduxjs/toolkit'
import type { User } from "../../components/Abstractions/User";

const initialState: User[] = [
    { 
        login: 'Admin', 
        password: 'Admin',
        username: 'Ivan Ivanov',
        email: 'Ivanov@mail.ru'  
    },
    { 
        login: 'Avdanin', 
        password: 'Avdanin',
        username: 'Yurii Avdanin',
        email: 'Avdanin@mail.ru'  
    }
  ];
  
  const usersSlice = createSlice({
    name: 'users',
    initialState,
    reducers: {
      addUser: (state, action: PayloadAction<User>) => {
        state.push(action.payload);
      }
    }
  });
  
  export const { addUser } = usersSlice.actions;
  export default usersSlice.reducer;