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
                        <a href="" className="link" onClick={(event) => this.onChange(event, "/NewNotification/Index")}>
                            <span className="icon-new-notification"></span>
                            <span className="text">New Notification</span>
                        </a>
                    </li>
                    <li className="menu">
                        <a href="" className="link" onClick={(event) => this.onChange(event, "/NotificationList/Index")}>
                            <span className="icon-notification-list"></span>
                            <span className="text">Notification List</span>
                        </a>
                    </li>
                    <li className="menu">
                        <a href="" className="link" onClick={(event) => this.onChange(event, "/NotificationTemplate/Index")}>
                            <span className="icon-template"></span>
                            <span className="text">Notification Template</span>
                        </a>
                    </li>
                    <li className="menu">
                        <a href="" className="link" onClick={(event) => this.onChange(event, "/ChatGroupMapping/Index")}>
                            <span className="icon-group-mapping"></span>
                            <span className="text">Chat Group Mapping</span>
                        </a>
                    </li>
                    <li className="menu">
                        <a href="" className="link" onClick={(event) => this.onChange(event, "/NotificationType/Index")}>
                            <span className="icon-notification-type"></span>
                            <span className="text">Notification Type</span>
                        </a>
                    </li>
                    <li className="menu">
                        <a href="" className="link" onClick={(event) => this.onChange(event, "/NewNotificationType/Index")}>
                            <span className="icon-notification-type"></span>
                            <span className="text">New Notification Type</span>
                        </a>
                    </li>
               </ul>
            </div>
        )
    }
}

export default connect()(Sidebar);
