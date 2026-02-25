import axios from 'axios';
import HttpClient from '../services/httpClient';
import React, { useEffect, useState } from 'react';
import { Button } from '@headlessui/react'
import { useNavigate } from 'react-router-dom';
import { LockOpenIcon } from '@heroicons/react/24/outline';
import FormRow from '../elements/FormRow';
import ValidationMessages from '../elements/ValidationMessages';

const ResetPassword: React.FC<{ token: string, handleLandingPageChange: (path: string) => void }> = ({ token, handleLandingPageChange }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [passwordCheck, setPasswordCheck] = useState('');
    const [showMessage, setShowMessage] = useState(false);
    const [showErrors, setShowErrors] = useState(false);
    const [errors, setErrors] = useState<{ [key: string]: string[] }>({});
    const [loading, setLoading] = useState(false);
    const [shouldNavigateToLogin, setShouldNavigateToLogin] = useState<boolean>(false);
    const navigate = useNavigate();

    function handleError(error: unknown) {
        setShowMessage(false);
        if (axios.isAxiosError(error)) {
            if (error.response && error.response.status === 400) {
                const errors = error.response.data.errors;
                setErrors(errors);
            } else if (error.response && error.response.status >= 500) {
                setErrors({ 'Server Error': ['An internal server error occurred.'] });
            } else {
                setErrors({ 'Server Error': [error.message] });
            }
        } else {
            setErrors({ 'Server Error': ['An unknown error occurred.'] });
        }
        setShowErrors(true);
    }
    function showSuccess() {
        setShowMessage(true);
        setShowErrors(false);
        setErrors({});
    }
    useEffect(() => {
        if (shouldNavigateToLogin) {
            setTimeout(() => {
                handleLandingPageChange('login');
                navigate('/', { replace: true });
            }, 4000);
        }
    }, [shouldNavigateToLogin]);

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setLoading(true);
        setErrors({});

        try {
            const encodedToken = encodeURIComponent(token);
            await new HttpClient().put('auth/reset-password', null, { token: encodedToken, email, password, passwordCheck });
            showSuccess();
            setShouldNavigateToLogin(true);
        } catch (error) {
            handleError(error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className='grid h-screen place-items-center'>
            <form onSubmit={handleSubmit}>
                <h2>Reset Password</h2>
                <div className='mx-auto px-3 bg-primary/5 w-96 rounded-lg py-2'>
                    <table className='w-full'>
                        <tbody>
                            <FormRow label="Email:" type="text" id="email" name="email" value={email} onChange={(e) => setEmail(e.target.value)} />
                            <FormRow label="Password:" type="password" id="password" name="password" value={password} onChange={(e) => setPassword(e.target.value)} />
                            <FormRow label="Confirm Password:" type="password" id="passwordCheck" name="passwordCheck" value={passwordCheck} onChange={(e) => setPasswordCheck(e.target.value)} />
                        </tbody>
                    </table>
                    <Button type="submit" className="btn btn-primary w-full btn-sm mt-2 " disabled={loading}>Reset Password
                        <LockOpenIcon className='w-4 h-4' />
                    </Button>
                    {showMessage && <p className="alert alert-success mt-2 text-sm">'Password reset successful.  Redirecting to Login page.  Please log in with your new password.'</p>}
                    <ValidationMessages showErrors={showErrors} errors={errors} />
                </div>
            </form>
        </div>
    );
};

export default ResetPassword;