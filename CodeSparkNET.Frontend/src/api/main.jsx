import React from 'react';
import ReactDOM from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import App from './server/App';
import '../assets/css/root.css';

const basename = process.env.NODE_ENV === 'production'
  ? '/CodeSpark'
  : '/';

ReactDOM.createRoot(document.getElementById('root')).render(
  <BrowserRouter basename={basename}>
    <App />
  </BrowserRouter>
);