import axios from 'axios';
import HttpClient from '../services/httpClient';
import { useState, FormEvent, FC } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@headlessui/react'
import { LockClosedIcon, UserPlusIcon, ArrowRightEndOnRectangleIcon } from '@heroicons/react/24/outline';
import ValidationMessages from '../elements/ValidationMessages';
import FormRow from '../elements/FormRow';

interface LoginProps {
  handleLandingPageChangeDelegate: (path: string) => void;
}

const Login: FC<LoginProps> = ({ handleLandingPageChangeDelegate: onLandingPageChange }) => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errors, setErrors] = useState<{ [key: string]: string[] }>({});
  const [showErrors, setShowErrors] = useState(false);
  const [disableLoginButton, setDisableLoginButton] = useState(false);
  const navigate = useNavigate();

  function handleError(error: unknown) {
    if (axios.isAxiosError(error)) {
      if (error.response && error.response.status === 400) {
        const validationErrors = error.response.data.errors;
        setErrors(validationErrors);
        setShowErrors(true);
      } else {
        console.error('There was a problem with the login:', error);
        setErrors({ "Server error": [error.message] });
        setShowErrors(true);
      }
    }else{
      console.error('An unexpected error occurred:', error);
      setErrors({ "Server error": ["There was a problem with the login."] });
      setShowErrors(true);
    }
  }

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setDisableLoginButton(true);

    const httpClient = new HttpClient();
    try {
      const response = await httpClient.post(`auth/signin`, { email, password });
      if (response.headers['x-auth-token']) {
        console.log('Received login token.');
        httpClient.setAuthToken(response.headers['x-auth-token']);
        setErrors({});
        setShowErrors(false);
        if (response.status === 200) {
          navigate('/market-data');
        }
      } else {
        setErrors({ "Server error": ["Then login was successful, but there was a problem transferring the login token."] });
        setShowErrors(true);
      }
    } catch (error) {
      // Handle the error (e.g., show an error message to the user)
      handleError(error);
      setDisableLoginButton(false);
    }
  };

  return (
    <div className='grid h-screen place-items-center'>
      <form onSubmit={handleSubmit}>
        <h2>Login</h2>
        <div className='mx-auto px-3 bg-primary/5 w-96 rounded-lg'>
          <table className='w-full'>
            <tbody>
              <FormRow label="Email" type="text" id="email" name="email" value={email} onChange={(e) => setEmail(e.target.value)} />
              <FormRow label="Password" type="password" id="password" name="password" value={password} onChange={(e) => setPassword(e.target.value)} />
            </tbody>
          </table>
          <div className='justify-center pt-2 '>
            <Button type="submit" className="btn btn-primary w-full btn-sm" disabled={disableLoginButton}>Login
              <ArrowRightEndOnRectangleIcon className='w-6 h-6' />
            </Button>
          </div>
          <div>
            <ValidationMessages showErrors={showErrors} errors={errors} />
          </div>
          <div className='flex justify-between py-2'>
            <Button className="btn btn-secondary btn-outline btn-xs font-light" onClick={() => { onLandingPageChange('signup'); }}>Sign Up
              <UserPlusIcon className='w-4 h-4' />
            </Button>
            <Button className="btn btn-secondary btn-outline btn-xs font-light" onClick={() => { onLandingPageChange('forgottenpassword'); }}>Forgot password?
              <LockClosedIcon className='w-4 h-4' />
            </Button>
          </div>
        </div>
      </form>
    </div>
  );
};

export default Login;