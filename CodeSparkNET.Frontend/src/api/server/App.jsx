import React from 'react';
import { Routes, Route } from 'react-router-dom';
import '../../assets/css/layout.css';
import Layout from '../../components/Layout';
import Index from '../../pages/Index';
import Privacy from '../../pages/Privacy';

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>

        <Route index element={<Index />} />
        <Route path="privacy" element={<Privacy />} />

      </Route>
    </Routes>
  );
};

export default App;