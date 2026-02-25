// RaysonCVUI/src/components/FormRow.tsx
import React from 'react';

interface FormRowProps {
    label: string;
    type: string;
    id: string;
    name: string;
    value: string;
    onChange: (e: React.ChangeEvent<HTMLInputElement>) => void;
}

const FormRow: React.FC<FormRowProps> = ({ label, type, id, name, value, onChange, }) => {
    return (
        <tr>
            <td className="text-left">
                <label htmlFor={id} className="form-label">{label}</label>
            </td>
            <td>
                <input type={type} id={id} name={name} value={value} onChange={onChange} className="input input-sm mt-2 w-full" />
            </td>
        </tr>
    );
};

export default FormRow;
