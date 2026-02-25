import { HashRouter, Route, Routes } from 'react-router-dom'
import LandingPage from './pages/LandingPage';
import BasicPage from './pages/Basic';
import ChatbotPage from './pages/ChatbotPage';
import './index.css'
import './App.css'
import './styles/tailwind.css'

function App() {
  return (
    <div className='secondlevelofhell'>
      <HashRouter>
        <Routes>
          {`Hi there`}
          <Route path="/" element={<LandingPage />} />
          <Route path="/chatbot" element={<ChatbotPage />} />
          <Route path="/dashboard" element={<BasicPage />} />
          {`Hi there 2`}
        </Routes>
      </HashRouter>
    </div>
  )
}

export default App
