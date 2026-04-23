import React from "react";
import { Routes, Route } from "react-router-dom";

import Login from "./layouts/Login";
import Signup from "./layouts/Signup";
import Homepage from "./layouts/Homepage";

function App() {
  return (
    <div className="App">

      <Routes>
        {/* Login Page */}
        <Route path="/" element={<Login />} />

        {/* Signup Page */}
        <Route path="/signup" element={<Signup />} />

        {/* Homepage (optional after login) */}
        <Route path="/home" element={<Homepage />} />
      </Routes>

    </div>
  );
}

export default App;