import { Card, Form, Button, Alert } from 'react-bootstrap';
import type { AuthFormProps } from "../Abstractions/Props/AuthFormProps";
import type { RootState } from '../../store';
import { useSelector } from 'react-redux';
import './_Styles.css';


const AuthCard = ({ fromData, onChange, onSubmit, onRedirectRegestry }: AuthFormProps) => {
    const error = useSelector((state: RootState) => state.auth.error);

    return (
        <Card className='Card'>
          <Card.Body>
            <Card.Title as="h2" className="text-center mb-4">
              Авторизация пользователя
            </Card.Title>
            
            <Form onSubmit={onSubmit}>
              <Form.Group className="mb-3" controlId="login">
                <Form.Label className="text-start w-100">Логин:</Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Введите логин"
                  name="login"
                  value={fromData.login}
                  onChange={onChange}
                  required
                />
              </Form.Group>

              <Form.Group className="mb-3" controlId="password">
                <Form.Label className="text-start w-100">Пароль:</Form.Label>
                <Form.Control
                  type="password"
                  placeholder="Введите пароль"
                  name="password"
                  value={fromData.password}                  
                  onChange={onChange}
                  required
                />
              </Form.Group>
              
              <Form.Group className="d-flex justify-content-end gap-2">
                <Button variant="outline-secondary" type="button" className="w-80" onClick={onRedirectRegestry} >
                  Регистрация
                </Button>
                <Button variant="primary" type="submit" className="w-100">
                  Войти
                </Button>
              </Form.Group>
            </Form>

            {error && <Alert className="Alert" variant="danger">{error}</Alert>}
          </Card.Body>
        </Card>
      );
};
    
export default AuthCard;

