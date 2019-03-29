import React, { Component } from 'react';
import { connect } from 'react-redux';
import AppData from '../../lib/AppData';
import Site from '../../lib/site';

import Logo from "../../../images/Logo.svg";
import "./Sidebar.scss";

class Sidebar extends Component {
    constructor(props){
        super(props);
        this.onChange = this.onChange.bind(this);
    }

    getMenuClassName = (name) => {
        return 'icon-' + name.replace(/ /g, '-').toLowerCase();
    }

    onChange(e, url){
        e.preventDefault();        
        this.props.onChange(Site.resolveClientUrl(url)); 
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
                    {
                        AppData.menus.map((menu, menuIndex) => 
                            <span className="menu" key={menuIndex}>
                                <span className="menu-text">{menu.Name}</span>
                                <ul className="submenu list-unstyled">
                                    {
                                        menu.SubMenus.map((submenu, submenuIndex ) => 
                                            <li className="menu-item" key={submenuIndex}>
                                                <a href="" className="link" onClick={(event) => this.onChange(event, submenu.Url)}>
                                                    <span className={this.getMenuClassName(submenu.Name)} />
                                                    <span className="text">{submenu.Name}</span>
                                                </a>
                                            </li>  
                                        )
                                    }
                                </ul>                                
                            </span> 
                        )
                    }                    
                </li>
               </ul>
            </div>
        );
    }
}

export default connect()(Sidebar);
