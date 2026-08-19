import type { UserFormInputData } from "../UserFormInputData";

export interface RegisterFromProps {
    formData: UserFormInputData;    
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
    onSubmit: (e: React.FormEvent) => void;
    error: string;
    success: boolean;
}