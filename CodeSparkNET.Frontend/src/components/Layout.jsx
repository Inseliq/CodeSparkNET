import React, { useEffect } from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import HeaderSite from './HeaderSite';
import HeaderApp from './HeaderApp';
import FooterSite from './FooterSite';
import FooterApp from './FooterApp';
import { setTitle } from '../app/utils/titleController';

const Layout = () => {
  const location = useLocation();

  useEffect(() => {
    setTitle(location.pathname, '/CodeSpark');

    if (window.reactAppReady) {
      window.reactAppReady();
    }
  }, [location.pathname]);

  return (
    <>
      <HeaderSite />
      <HeaderApp />
      <main className="component">
        <Outlet />
      </main>
      <FooterApp />
      <FooterSite />
    </>
  );
};

export default Layout;