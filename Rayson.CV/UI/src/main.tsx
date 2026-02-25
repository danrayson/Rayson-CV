import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import ErrorBoundary from './components/ErrorBoundary'
import { initializeErrorHandlers } from './utils/errorHandling'
import './index.css'

initializeErrorHandlers();

var element = document.getElementById('root')
if(element){
  element.className = "yummyroot"
}
createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ErrorBoundary>
      <App />
    </ErrorBoundary>
  </StrictMode>,
)
