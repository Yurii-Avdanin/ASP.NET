import 'bootstrap/dist/css/bootstrap.min.css';
import { Routes, Route } from 'react-router-dom';
import HomePage from './components/Pages/HomePage';
import Login from './components/Pages/PageLoginUser';
import PageRegisterUser from './components/Pages/PageRegisterUser';
import NotFound from './components/Pages/NotFound';
import { IsAuth } from './components/HOC/IsAuth';
import { IsAuthUser } from './components/HOC/IsAuthUser';
import Headline from './components/Panels/Headline';

const CurrentPage = IsAuth(HomePage);
const LoginUser = IsAuthUser(Login);
const RegisterUser = IsAuthUser(PageRegisterUser);

const AppRoutes = () => (
  <div>    
    <Headline text={'Домашняя работа №7'}/>  
    <Routes>
      <Route path="/" element={<CurrentPage/> } />      
      <Route path="/login" element={<LoginUser/> } />
      <Route path="/register" element={<RegisterUser/> } />      
      <Route path="*" element={<NotFound />} />
    </Routes>
  </div>
);

export default AppRoutes;