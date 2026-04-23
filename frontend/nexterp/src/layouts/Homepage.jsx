import React from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import '../css/HomepageCss.css';
import back from '../assests/Images/100.jpg';
import Nexterp from '../assests/Images/Nexterp_bg_removed.png'

function Homepage() {
    return (
        <div className="BannerContainer">
            <img src={back} className="Banner" alt="Background" />

            <div className="Overlay"></div>

            <div className="OpenContent">
                <div className="OpenCard">
                      <img src={Nexterp} className="DescContainer" alt="Logo" />
                    <h2 className="title">Welcome</h2>
                    <p className="subtitle">Manage your organization with us
                        And find the organized solution for your organization </p>

                    <div className="buttonGroup">
                        <button className="btn custom-btn-primary w-100 mb-3">
                            Open Organization
                        </button>
                        <button className="btn custom-btn-secondary w-100">
                            Create Organization
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Homepage;