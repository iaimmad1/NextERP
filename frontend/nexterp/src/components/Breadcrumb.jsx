import React from "react";
import 'bootstrap/dist/css/bootstrap.min.css';
import 'bootstrap/dist/js/bootstrap.bundle.min.js';

function Breadcrumb() {
    return (
        <div>
            <h1>This is breadcrumb Section</h1>

            <nav 
                style={{ '--bs-breadcrumb-divider': "'>'" }} 
                aria-label="breadcrumb"
            >
                <ol className="breadcrumb">
                    <li className="breadcrumb-item">
                        <a href="#">Home</a>
                    </li>
                    <li className="breadcrumb-item active" aria-current="page">
                        Library
                    </li>
                </ol>
            </nav>

        </div>
    );
}

export default Breadcrumb;