import { Card, Form, Button, Alert } from 'react-bootstrap';
import type { RegisterFromProps } from "../Abstractions/Props/RegisterFromProps";
import './_Styles.css';

const RegistrationCard = ({ formData, onChange, onSubmit, error, success }: RegisterFromProps) => {
  return (
    <Card className='Card'>
      <Card.Body>
        <Card.Title as="h2" className="text-center mb-4">
          Регистрация пользователя
        </Card.Title>

        {error && <Alert variant="danger">{error}</Alert>}
        {success && <Alert variant="success">Регистрация успешна!</Alert>}

        <Form onSubmit={onSubmit}>
          <Form.Group className="mb-3" controlId="login">
            <Form.Label className="text-start w-100">Логин:</Form.Label>
            <Form.Control
              type="text"
              placeholder="Введите логин"
              name="login"
              value={formData.login}              
              onChange={onChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="username">
            <Form.Label className="text-start w-100">Имя пользователя:</Form.Label>
            <Form.Control
              type="text"
              placeholder="Введите имя"
              name="username"
              value={formData.username}              
              onChange={onChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="email">
            <Form.Label className="text-start w-100">Email:</Form.Label>
            <Form.Control
              type="email"
              placeholder="Введите email"
              name="email"
              value={formData.email}
              onChange={onChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="password">
            <Form.Label className="text-start w-100">Пароль:</Form.Label>
            <Form.Control
              type="password"
              placeholder="Придумайте пароль"
              name="password"
              value={formData.password}
              onChange={onChange}
              required
            />
          </Form.Group>

          <Form.Group className="mb-3" controlId="confirmPassword">
            <Form.Label className="text-start w-100">Подтверждение пароля:</Form.Label>
            <Form.Control
              type="password"
              placeholder="Повторите пароль"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={onChange}
              required
            />
          </Form.Group>

          <Button variant="outline-primary" type="submit" className="w-100">
            Зарегистрироваться
          </Button>
        </Form>
      </Card.Body>
    </Card>
  );
};

export default RegistrationCard;