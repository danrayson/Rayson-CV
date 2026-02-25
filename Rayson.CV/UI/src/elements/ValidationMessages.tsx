import React from 'react';

interface ValidationMessagesProps {
  showErrors: boolean;
  errors: { [key: string]: string[] };
}

const ValidationMessages: React.FC<ValidationMessagesProps> = ({ showErrors, errors }) => {
  if (!showErrors || Object.keys(errors).length === 0) {
    return null;
  }

  return (
    <div>
      {Object.keys(errors).map((key, index) => (
        <div key={index}>
          {errors[key].map((errorMessage, errorIndex) => (
            <div key={errorIndex} className="alert alert-outline mt-2 text-sm">
              {errorMessage}
            </div>
          ))}
        </div>
      ))}
    </div>
  );
};

export default ValidationMessages;
