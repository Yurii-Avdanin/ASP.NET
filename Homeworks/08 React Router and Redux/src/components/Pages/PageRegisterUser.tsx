import React, { useState, useMemo } from 'react';
import { useSelector, useDispatch } from 'react-redux';
import { useNavigate } from 'react-router-dom';
import { Container} from 'react-bootstrap';
import RegistrationCard from '../Cards/RegistrationCard'; 
import { type UserFormInputData } from "../Abstractions/UserFormInputData";
import { type User } from "../Abstractions/User";
import { type RootState } from '../../store';
import { addUser } from '../../store/Reducers/usersSlice';
import { type ButtonForm } from '../Abstractions/ButtonForm';
import ShowModal from '../ModalForms/ShowModal';

const PageRegisterUser: React.FC = () => {
  const [formInputData, setFormInputData] = useState<UserFormInputData>({
    login: '',
    username: '',
    email: '',
    password: '',
    confirmPassword: ''
  });

  const dispatch = useDispatch();
  const navigate = useNavigate();
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

    const newUser: User = {
      login: formInputData.login,
      password: formInputData.password,
      username: formInputData.username,
      email: formInputData.email
    };

    dispatch(addUser(newUser));

    console.log('Пользователь зарегистрирован.  object (user):', newUser);
    setShowModal(true);
  };

  const [showModal, setShowModal] = useState<boolean>(false);

  const handleRedirect = () => {
    setShowModal(false);   
    setFormInputData({
      login: '',
      username: '',
      email: '',
      password: '',
      confirmPassword: ''});

    navigate('/login');
  };  

  const handleCloseModal = () => {
    setShowModal(false);
    setFormInputData({
      login: '',
      username: '',
      email: '',
      password: '',
      confirmPassword: ''});
  };
  
  const [buttonOk] = useState<ButtonForm>({Caption: 'Вход', onClick: handleRedirect});
  const [buttonClose] = useState<ButtonForm>({Caption: 'Закрыть', onClick: handleCloseModal});

  const messages = [
    `Пользователь "${formInputData.login}" успешно зарегистрирован!`,
    'Теперь вы можете войти в систему.'    
  ];

  return (    
    <Container>
      <RegistrationCard
        formData={formInputData}
        onChange={handleChange}
        onSubmit={handleSubmit}
        error={error}
        success={success}
      />      
      <ShowModal
        messages={messages}
        buttonClose={buttonClose}
        buttonOk={buttonOk}
        showModal={showModal}
      />
    </Container>    
  );
};

export default PageRegisterUser;