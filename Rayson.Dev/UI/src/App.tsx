import { HashRouter, Route, Routes } from 'react-router-dom'
import LandingPage from './pages/LandingPage';
import MarketPricesPage from './pages/MarketPricesPage'; // Import the new page
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
          <Route path="/market-data" element={<MarketPricesPage />} />
          {/* Add this line */}
          {`Hi there 2`}
        </Routes>
      </HashRouter>
    </div>
  )
}

export default App
