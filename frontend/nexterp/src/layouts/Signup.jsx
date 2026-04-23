import React, { useState } from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import '../css/LoginCss.css';

import back from '../assests/Images/100.jpg';
import logo from '../assests/Images/Nexterp_bg_removed.png';
import show from '../assests/Icons/show.svg';
import hide from '../assests/Icons/hide.svg';
import { useNavigate } from "react-router-dom";

function Signup() {

    const navigate = useNavigate();

    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);

    const [form, setForm] = useState({
        organizationName: "",
        pan: "",
        name: "",
        email: "",
        phone: "",
        password: "",
        confirmPassword: ""
    });

    const handleChange = (e) => {
        const { name, value } = e.target;

        let updatedValue = value;

        if (name === "organizationName") {
            updatedValue = value.slice(0, 60);
        }

        if (name === "name") {
            updatedValue = value.slice(0, 30);
        }

        if (name === "pan") {
            updatedValue = value.replace(/[^0-9]/g, "").slice(0, 12);
        }

        if (name === "phone") {
            updatedValue = value.replace(/[^0-9]/g, "").slice(0, 10);
        }

        setForm({ ...form, [name]: updatedValue });
    };

    const passwordMismatch =
        form.confirmPassword.length > 0 &&
        form.password !== form.confirmPassword;

    return (
        <div className="LoginContainer">

            <img src={back} className="LoginBg" alt="Background" />
            <div className="Overlay"></div>

            <div className="LoginCard">

                <div className="LoginLeft">
                    <img src={logo} className="LoginLogo" alt="Logo" />
                </div>

                <div className="LoginRight">

                    <h2 className="login-title">Create Account</h2>
                    <p className="login-subtitle">Sign up to get started</p>

                    <form>

                        <div className="mb-3">
                            <input
                                name="organizationName"
                                type="text"
                                className="form-control custom-input"
                                placeholder="Organization Name"
                                value={form.organizationName}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="mb-3">
                            <input
                                name="pan"
                                type="text"
                                className="form-control custom-input"
                                placeholder="PAN Number"
                                value={form.pan}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <h2 className="admin-title">Admin Information</h2>

                        <div className="mb-3">
                            <input
                                name="name"
                                type="text"
                                className="form-control custom-input"
                                placeholder="Full Name"
                                value={form.name}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="mb-3">
                            <input
                                name="email"
                                type="email"
                                className="form-control custom-input"
                                placeholder="Email"
                                value={form.email}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="mb-3">
                            <input
                                name="phone"
                                type="text"
                                className="form-control custom-input"
                                placeholder="Phone Number"
                                value={form.phone}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="mb-3">
                            <input
                                name="password"
                                type={showPassword ? "text" : "password"}
                                className="form-control custom-input"
                                placeholder="Password"
                                value={form.password}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="mb-2">
                            <input
                                name="confirmPassword"
                                type={showConfirmPassword ? "text" : "password"}
                                className="form-control custom-input"
                                placeholder="Confirm Password"
                                value={form.confirmPassword}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        {passwordMismatch && (
                            <small className="error-text">
                                Passwords do not match
                            </small>
                        )}

                        <button className="btn custom-login-btn w-100 mt-3">
                            Sign Up
                        </button>

                        <div className="register-wrapper mt-3">
                            <p className="register-text">
                                Already have an account?
                                <br />
                                <span
                                    className="register-link"
                                    onClick={() => navigate("/")}
                                    style={{ cursor: "pointer" }}
                                >
                                    Login now
                                </span>
                            </p>
                        </div>

                    </form>

                </div>

            </div>
        </div>
    );
}

export default Signup;