import React, { useState, useMemo } from 'react';
import { useDispatch } from 'react-redux';
import { loginSuccess, loginFailure, clearError } from '../../store/Reducers/authSlice';
import { useNavigate } from 'react-router-dom';
import { Container } from 'react-bootstrap';
import AuthCard from '../Cards/AuthCard'; 
import type { AuthFormInputData } from "../Abstractions/AuthFormInputData";
import type { RootState } from '../../store';
import { useSelector } from 'react-redux';

const PageLoginUser: React.FC = () => {  
  const [fromData, setFromData] = useState<AuthFormInputData>({login: '', password: ''});
  const dispatch = useDispatch();
  const navigate = useNavigate();
  const users = useSelector((state: RootState) => state.users);
  
  const foundUser = useMemo(
    () => users.find(user => user.login === fromData.login),
    [users, fromData.login]
  );

  let errorMasage: string = 'Неверные учетные данные';

  const validate = (): boolean => {
    console.log('Start validate!');
    //console.log('foundUser:', foundUser);
    if (foundUser)
    {      
      if (fromData.password === foundUser.password) 
      {        
        errorMasage = '';
        return true;
      }      
    }
    return false;
  };

  const handleLogin = (e: React.FormEvent) => {    
    e.preventDefault();

    if (validate()) 
    {
      console.log('validate = true');    
      dispatch(loginSuccess(fromData.login));
      navigate('/');
    } 
    else {      
      dispatch(loginFailure(errorMasage));      
    }
  };  

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFromData((prev) => ({ ...prev, [name]: value }));    
    dispatch(clearError());
  };

  return (
    <Container>
      <AuthCard
        fromData={fromData}        
        onChange={handleChange}
        onSubmit={handleLogin}
      />      
    </Container>
  );  
};

export default PageLoginUser;
