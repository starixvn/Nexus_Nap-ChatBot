import React, { Component } from 'react';
import { connect } from 'react-redux';
import Logo from "../../../images/Logo.svg";

import "./Sidebar.scss";

class Sidebar extends Component {
    constructor(props){
        super(props);
        this.onChange = this.onChange.bind(this);
    }

    onChange(e, url){
        e.preventDefault();
        console.log(url);
        this.props.onChange(url); 
    }
      render() {
        return (
            <div className="sidebar">
               <div className="nav-top">
                    <img src={Logo} className="logo" />
                    <h1 className="site-name">Chatbot</h1>
               </div>
               <ul className="nav">
                <li className="menu">
                    <span className="menu-text">SKype Chatbot</span>
                    <ul className="submenu list-unstyled">
                        <li className="menu-item">
                            <a href="" className="link" onClick={(event) => this.onChange(event, "/NewNotification/Index")}>
                                <span className="icon-new-notification"></span>
                                New Notification
                            </a>
                        </li>
                        <li className="menu-item">
                            <a href="" className="link" onClick={(event) => this.onChange(event, "/NotificationList/Index")}>
                                <span className="icon-notification-list"></span>
                                Notification List
                            </a>
                        </li>
                        <li className="menu-item">
                            <a href="" className="link" onClick={() => this.onChange("/NotificationTemplate/Index")}>
                                <span className="icon-template"></span>
                                Notification Template
                            </a>
                        </li>
                        <li className="menu-item">
                            <a href="" className="link" onClick={() => this.onChange("/ChatGroupMapping/Index")}>
                                <span className="icon-group-mapping"></span>
                                Chat Group Mapping
                            </a>
                        </li>
                        <li className="menu-item">
                            <a href="" className="link" onClick={() => this.onChange("/NotificationType/Index")}>
                                <span className="icon-notification-type"></span>
                                Notification Type
                            </a>
                        </li>
                    </ul>
                </li>
               </ul>
            </div>
        )
    }
}

export default connect()(Sidebar);
