import React, { useState, useMemo } from 'react';
import { Container} from 'react-bootstrap';
import RegistrationCard from '../Cards/RegistrationCard'; 
import type { UserFormInputData } from "../Abstractions/UserFormInputData";
import type { RootState } from '../../store';
import { useSelector } from 'react-redux';

const PageRegisterUser: React.FC = () => {
  const [formInputData, setFormInputData] = useState<UserFormInputData>({
    login: '',
    username: '',
    email: '',
    password: '',
    confirmPassword: ''
  });

  const [error, setError] = useState<string>('');
  const [success, setSuccess] = useState<boolean>(false);
  const users = useSelector((state: RootState) => state.users);  
  const foundUser = useMemo(
    () => users.find(user => user.login === formInputData.login),
    [users, formInputData.login]
  );

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormInputData((prev) => ({ ...prev, [name]: value }));    
    setError('');
    setSuccess(false);
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();

    if (!formInputData.login || !formInputData.username || !formInputData.email || !formInputData.password) {
      setError('Все поля обязательны для заполнения');
      return;
    }

    if (foundUser)
    {
      setError(`Логин \"${formInputData.login}\" уже занят`);
      return;
    }

    if (formInputData.password !== formInputData.confirmPassword) {
      setError('Пароли не совпадают');
      return;
    }

    if (formInputData.password.length < 3) {
      setError('Пароль должен содержать минимум 3 символов');
      return;
    }
    
    console.log('Пользователь зарегистрирован.  object:', formInputData);
    setSuccess(true);     
  };

  return (    
    <Container>
      <RegistrationCard
        formData={formInputData}
        onChange={handleChange}
        onSubmit={handleSubmit}
        error={error}
        success={success}
      />
    </Container>
  );
};

export default PageRegisterUser;