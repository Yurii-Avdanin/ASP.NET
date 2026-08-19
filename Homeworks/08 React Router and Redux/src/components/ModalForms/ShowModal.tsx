import type { ShowModalProps } from "../Abstractions/Props/ShowModalProps";
import { Modal, Button } from 'react-bootstrap';

const ShowModal = ({ messages,  buttonOk, buttonClose, showModal}: ShowModalProps) => {
    return (
        <Modal show={showModal} onHide={buttonClose.onClick}>
            
            <Modal.Header>
                <Modal.Title>Регистрация завершена</Modal.Title>
            </Modal.Header>
            
            <Modal.Body>
                {messages.map((msg, index) => (
                    <p key={index}>{msg}</p>
                ))}                
            </Modal.Body>

            <Modal.Footer>
                <Button variant="outline-secondary" onClick={buttonClose.onClick}>
                    {buttonClose.Caption}
                </Button>
                
                <Button variant="primary" onClick={buttonOk.onClick}>
                    {buttonOk.Caption}
                </Button>
            </Modal.Footer>
        </Modal>
    )
};

export default ShowModal;