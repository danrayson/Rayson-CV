import Login from '../components/Login';
import SignUp from '../components/SignUp';
import ForgottenPassword from '../components/ForgottenPassword';
import ResetPassword from '../components/ResetPassword';
import { useEffect, useState } from 'react';

const LandingPage: React.FC = () => {
  const [currentPage, setCurrentPage] = useState('login');
  const [token, setToken] = useState('');
  //useLocation not working inside App's HashRouter.  Use window.location instead.
  const location = window.location;

  useEffect(() => {
    // Extract query parameters
    const queryParams = new URLSearchParams(location.search);
    const token = queryParams.get('token');
    if (token) {
      setToken(token);
      setCurrentPage('resetpassword')
    }
  }, [location.search]);

  const handleLandingPageChange = (page: string) => {
    setCurrentPage(page);
  };

  const componentsMap: { [key: string]: React.ReactNode } = {
    "login": <Login handleLandingPageChangeDelegate={(path: string) => handleLandingPageChange(path)} />,
    "signup": <SignUp handleLandingPageChange={(path: string) => handleLandingPageChange(path)} />,
    "forgottenpassword": <ForgottenPassword handleLandingPageChange={(path: string) => handleLandingPageChange(path)} />,
    "resetpassword": <ResetPassword token={token} handleLandingPageChange={(path: string) => handleLandingPageChange(path)} />
  };

  return (
    <div>
      {componentsMap[currentPage]}
    </div>
  );
};

export default LandingPage;
