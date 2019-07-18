import React from 'react';
import AppData from '../../../lib/AppData';
import Site from '../../../lib/site';

import './Sidebar.scss';

export default class Sidebar extends React.Component {
    constructor(props) {
        super(props);
    }

    getMenuClassName = name => 'icon-' + name.replace(/ /g, '-').toLowerCase();

    getMenu() {
        const menu = AppData ? AppData.menus : null;
        if (!menu || menu.length === 0) {
            return [];
        }
        return menu;
    }

    onChange = (e, url) => {
        e.preventDefault();
        this.props.onChange(url);
    }

    renderMenu() {
        const menu = this.getMenu();
        return menu.map((menuItem, menuIndex) => this.renderMenuItem(menuItem, menuIndex));
    }

    renderMenuItem(menuItem, menuIndex) {
        return (<span className="menu" key={menuIndex}>
            <ul className="submenu list-unstyled">
                {
                    menuItem.SubMenus.map((submenu, submenuIndex) =>
                        <li className="menu-item" key={submenuIndex}>
                            <a href="" className="link" onClick={event => this.onChange(event, submenu.Url)}>
                                <span className={this.getMenuClassName(submenu.Name)} />
                                <span className="text">{submenu.Name}</span>
                            </a>
                        </li>
                    )
                }
            </ul>
        </span>);
    }

    render() {
        return (
            <div className="sidebar">
                <div className="nav-top">
                    <img src={Site.resolveClientUrl('images/Logo.svg')} className="logo" />
                    <h1 className="site-name">Chatbot</h1>
                </div>
                <ul className="nav">
                    <li className="menu">
                        {this.renderMenu()}
                    </li>
                </ul>
            </div>
        );
    }
}