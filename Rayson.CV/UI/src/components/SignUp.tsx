import React, { useEffect, useState } from 'react';
import axios from 'axios';
import HttpClient from '../services/httpClient';
import { Button } from '@headlessui/react'
import { LockClosedIcon, UserPlusIcon, ArrowRightEndOnRectangleIcon } from '@heroicons/react/24/outline';
import ValidationMessages from '../elements/ValidationMessages';
import FormRow from '../elements/FormRow';

interface SignUpProps {
  handleLandingPageChange: (page: string) => void;
}

const SignUp: React.FC<SignUpProps> = ({ handleLandingPageChange: handleLandingPageChange }) => {
  const [displayName, setDisplayName] = useState('');
  const [email, setEmail] = useState('');
  const [errors, setErrors] = useState<{ [key: string]: string[] }>({});
  const [showErrors, setShowErrors] = useState(false);
  const [password, setPassword] = useState('');
  const [showSuccessMessage, setShowSuccessMessage] = useState(false);

  function handleError(error: unknown) {
    if (axios.isAxiosError(error)) {
      console.error('Error signing up:', error.response ? error.response.data : error.message);

      if (error.response && error.response.status === 400) {
        setErrors(error.response.data.errors);
        setShowErrors(true);
      } else {
        console.error('There was a problem with the signup:', error);
        setErrors({ "Server error": [error.message] });
        setShowErrors(true);
      }
    } else {
      console.error('An unexpected error occurred:', error);
      setErrors({ "Server error": ["There was a problem with the signup."] });
      setShowErrors(true);
    }
  }
  function showSuccess() {
    setErrors({});
    setShowErrors(false);
    setShowSuccessMessage(true);
  }

  useEffect(() => {
    if (showSuccessMessage) {
      setTimeout(() => {
        setShowSuccessMessage(false);
        handleLandingPageChange('login');
      }, 4000);
    }
  }, [showSuccessMessage, handleLandingPageChange]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const httpClient = new HttpClient();
    try {
      await httpClient.post(`auth/signup`, { displayName, email, password, });
      showSuccess();
    } catch (error) {
      handleError(error);
    }
  };

  return (
    <div className='grid h-screen place-items-center'>
      <form onSubmit={handleSubmit}>
        <h2>Sign Up</h2>
        <div className='mx-auto px-3 bg-primary/5 w-96 rounded-lg'>
          <table className='w-full'>
            <tbody>
              <FormRow label="Display Name:" type="text" id="displayName" name="displayName" value={displayName} onChange={(e) => setDisplayName(e.target.value)} />
              <FormRow label="Email:" type="text" id="email" name="email" value={email} onChange={(e) => setEmail(e.target.value)} />
              <FormRow label="Password:" type="password" id="password" name="password" value={password} onChange={(e) => setPassword(e.target.value)} />
            </tbody>
          </table>
          <Button type="submit" className="btn btn-primary w-full btn-sm mt-2">Sign Up
            <ArrowRightEndOnRectangleIcon className='w-6 h-6' />
          </Button>
          <ValidationMessages showErrors={showErrors} errors={errors} />
          {showSuccessMessage && <p className="alert alert-success mt-2 text-sm">Successfully signed up. Redirecting to Login page.</p>}
          <div className='flex justify-between py-2'>
            <Button className="btn btn-secondary btn-outline btn-xs font-light" onClick={() => { handleLandingPageChange('login'); }}>Log In
              <UserPlusIcon className='w-4 h-4' />
            </Button>
            <Button className="btn btn-secondary btn-outline btn-xs font-light" onClick={() => { handleLandingPageChange('forgottenpassword'); }}>Forgot password?
              <LockClosedIcon className='w-4 h-4' />
            </Button>
          </div>
        </div>
      </form>
    </div>
  );
};

export default SignUp;