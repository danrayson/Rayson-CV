import axios from 'axios';
import HttpClient from '../services/httpClient';
import React, { useState } from 'react';
import { Button } from '@headlessui/react'
import { LockOpenIcon, UserPlusIcon, ArrowRightEndOnRectangleIcon } from '@heroicons/react/24/outline';
import FormRow from '../elements/FormRow';
import ValidationMessages from '../elements/ValidationMessages';

const ForgottenPassword: React.FC<{ handleLandingPageChange: (path: string) => void }> = ({ handleLandingPageChange }) => {
  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');
  const [errors, setErrors] = React.useState<{ [key: string]: string[] }>({});
  const [shouldShowMessage, setShouldShowMessage] = useState(false);
  const [shouldShowErrors, setShouldShowErrors] = useState(false);
  const [disableSendEmailButton, setDisableSendEmailButton] = useState(false);

  function showErrors(validationErrors: {[key: string]: string[]}) {
    setMessage('');
    setErrors(validationErrors);
    setShouldShowErrors(true);
  }
  function showMessage(message:string) {
    setMessage(message);
    setErrors({});
    setShouldShowMessage(true);
    setShouldShowErrors(false);
  }
  function handleError(error: unknown) {
    if (axios.isAxiosError(error)) {
      if (error.response && error.response.status === 400) {
        const validationErrors = error.response.data.errors;
        showErrors(validationErrors);
        setDisableSendEmailButton(false);
      } else {
        console.error('There was a problem with the password reset request:', error);
        showErrors({ "Server Error": [error.message] });
        setDisableSendEmailButton(false);
      }
    }
  }
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setDisableSendEmailButton(true);
    const httpClient = new HttpClient();
    try {
      const websiteUrl = window.location.origin; // This will get the current page's origin, including protocol and port if any
      await httpClient.post(`auth/request-password-reset`, { email, websiteUrl });
      showMessage('Password reset token sent successfully!');
    } catch (error) {
      handleError(error);
    }
  };

  return (
    <div className='grid h-screen place-items-center'>
      <form onSubmit={handleSubmit}>
        <h2>Forgot Password?</h2>
        <div className='mx-auto px-3 bg-primary/5 w-96 rounded-lg'>
          <table className='w-full'>
            <tbody>
              <FormRow label="Email:" type="text" id="email" name="email" value={email} onChange={(e) => setEmail(e.target.value)} />
            </tbody>
          </table>
          <Button type="submit" className="btn btn-primary w-full btn-sm mt-2" disabled={disableSendEmailButton}>Send Reset Link
            <LockOpenIcon className='w-6 h-6' />
          </Button>
          <ValidationMessages showErrors={shouldShowErrors} errors={errors} />
          {shouldShowMessage && <p className="alert alert-success mt-2 text-sm">{message}</p>}
          <div className='flex justify-between py-2'>
            <Button className="btn btn-secondary btn-outline btn-xs font-light" onClick={() => { handleLandingPageChange('signup'); }}>Sign Up
              <UserPlusIcon className='w-4 h-4' />
            </Button>
            <Button className="btn btn-secondary btn-outline btn-xs font-light" onClick={() => { handleLandingPageChange('login'); }}>Log In?
              <ArrowRightEndOnRectangleIcon className='w-4 h-4' />
            </Button>
          </div>
        </div>
      </form>
    </div >
  );
};

export default ForgottenPassword;
