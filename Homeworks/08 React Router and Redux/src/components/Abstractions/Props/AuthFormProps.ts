import type { AuthFormInputData } from "../AuthFormInputData";

export interface AuthFormProps {
    fromData: AuthFormInputData;    
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    onSubmit: (e: React.FormEvent) => void;    
}