import React from 'react';
import { BrowserRouter, Route, Routes } from 'react-router-dom';
import Login from './HomePage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/Auth/login" element={<Login />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;