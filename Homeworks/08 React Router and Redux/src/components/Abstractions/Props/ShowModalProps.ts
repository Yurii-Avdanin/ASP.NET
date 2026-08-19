import { type ButtonForm } from "../ButtonForm";

export interface ShowModalProps {
    messages: string[];
    buttonOk: ButtonForm;
    buttonClose: ButtonForm;    
    showModal: boolean;
}