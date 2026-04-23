import React, { useState } from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import '../css/LoginCss.css';

import back from '../assests/Images/100.jpg';
import logo from '../assests/Images/Nexterp_bg_removed.png';
import { useNavigate } from "react-router-dom";

function Login() {

    const navigate = useNavigate();

    return (
        <div className="LoginContainer">

            <img src={back} className="LoginBg" alt="Background" />
            <div className="Overlay"></div>

            <div className="LoginCard">

                <div className="LoginLeft">
                    <img src={logo} className="LoginLogo" alt="Logo" />
                </div>

                <div className="LoginRight">

                    <h2 className="login-title">Welcome Back</h2>
                    <p className="login-subtitle">Login to continue</p>

                    <form>

                        <div className="mb-3">
                            <input
                                type="email"
                                className="form-control custom-input"
                                placeholder="Email"
                                required
                            />
                        </div>

                        <div className="mb-3">
                            <input
                                type="password"
                                className="form-control custom-input"
                                placeholder="Password"
                                required
                            />
                        </div>

                        <button className="btn custom-login-btn w-100">
                            Login
                        </button>

                        <div className="register-wrapper mt-3">
                            <p className="register-text">
                                Don't have an account?
                                <br />
                                <span
                                    className="register-link"
                                    onClick={() => navigate("/signup")}
                                    style={{ cursor: "pointer" }}
                                >
                                    Register now
                                </span>
                            </p>
                        </div>

                    </form>

                </div>

            </div>
        </div>
    );
}

export default Login;