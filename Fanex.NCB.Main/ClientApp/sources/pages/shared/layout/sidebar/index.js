import React from 'react';
import axios from 'axios';
import Site from '../../../lib/site';

import './Sidebar.scss';

export default class Sidebar extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            menuItems: []
        };
    }

    componentDidMount() {
        axios.get(Site.resolveClientUrl('Auth/GetMenu'))
            .then(response => this.setState({ menuItems: response.data }));
    }

    getMenuClassName = name => 'icon-' + name.replace(/ /g, '-').toLowerCase();

    renderMenu = () => {
        return this.state.menuItems.map(this.renderMenuItem);
    }

    renderMenuItem = (menuItem, menuIndex) => {
        return (<span className="menu" key={menuIndex}>
            <ul className="submenu list-unstyled">
                {menuItem.subMenus.map(this.renderSubMenuItem)}
            </ul>
        </span>);
    }

    renderSubMenuItem = (submenu, submenuIndex) => (
        <li className="menu-item" key={submenuIndex}>
            <a href={Site.resolveClientUrl(submenu.url)} className="link" onClick={event => this.onChange(event, submenu.Url)}>
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
                <ul className="nav">
                    <li className="menu">
                        {this.renderMenu()}
                    </li>
                </ul>
            </div>
        );
    }
}