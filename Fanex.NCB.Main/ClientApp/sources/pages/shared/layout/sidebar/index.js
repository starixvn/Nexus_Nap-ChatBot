import React from 'react';
import axios from 'axios';
import Site from '../../../lib/site';

import './Sidebar.scss';

export default class Sidebar extends React.Component {
    constructor(props) {
        super(props);

        this.currentController = document.getElementById('current-controller') ?
            document.getElementById('current-controller').value.toLowerCase() : '';
        this.currentAction = document.getElementById('current-action') ?
            document.getElementById('current-action').value.toLowerCase() : '';

        this.state = {
            menuItems: []
        };
    }

    componentDidMount() {
        axios.get(Site.resolveClientUrl('User/GetMenu'))
            .then(response => this.setState({ menuItems: response.data }));
    }

    getMenuClassName = name => {
        switch (name) {
            case 'New Notification': return 'icon-new-notification';
            case 'Notification List': return 'icon-notification-list';
            case 'Notification Template': return 'icon-template';
            case 'Chat Group Mapping': return 'icon-group-mapping';
            case 'Notification Type': return 'icon-notification-type';
			case 'Setting Configuration': return 'icon-setting';
            case 'View Log': return 'icon-view-log';
        }
    }

    isCurrent = url => url.toLowerCase() === `/${this.currentController}${this.currentAction !== 'index' ? `/${this.currentAction}` : ''}`

    renderMenu = () => {
        return this.state.menuItems.map(menuItem => menuItem.subMenus.length > 0 ? this.renderSubMenu(menuItem) : this.renderMenuItem(menuItem));
    }

    isCurrentParent = url => url.toLowerCase().includes(`/${this.currentController}`)

    renderSubMenu = menuItem => (<li className={`menu-item ${this.isCurrentParent(menuItem.url) ? "" : " dropdown"}`} key={menuItem.id}>
        <div className="not-link-item dropdown-div">
            <span className={`span-icon ${this.getMenuClassName(menuItem.name)}`} />
            <span className="text">{menuItem.name}</span>
            <span className="icon-arrow-down"></span>
        </div>
        <ul className={`sub-menu menu${this.isCurrentParent(menuItem.url) ? "" : " dropdown-content"}`}>
            {menuItem.subMenus.map(this.renderMenuItem)}
        </ul>
    </li>);

    renderMenuItem = (submenu) => (
        <li className="menu-item" key={submenu.id}>
            <a href={Site.resolveClientUrl(submenu.url).replace('//', '/')} className={`link ${this.isCurrent(submenu.url) && 'current'} `}>
                <span className={this.getMenuClassName(submenu.name)} />
                <span className="text">{submenu.name}</span>
            </a>
        </li>
    )

    render() {
        return (
            <div className="sidebar">
                <div className="nav-top">
                    <img src={Site.resolveClientUrl('images/Logo.svg')} className="logo" />
                    <h1 className="site-name">Chatbot</h1>
                </div>
                <div className="nav">
                    <ul className="menu" >
                        {this.renderMenu()}
                    </ul>
                </div>
            </div>
        );
    }
}