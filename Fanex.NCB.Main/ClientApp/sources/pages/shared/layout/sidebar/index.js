import React from 'react';
import axios from 'axios';
import Site from '../../../lib/site';

import './Sidebar.scss';

export default class Sidebar extends React.Component {
    constructor(props) {
        super(props);

        this.currentController = document.getElementById('current-controller').value.toLowerCase();
        this.currentAction = document.getElementById('current-action').value.toLowerCase();

        this.state = {
            menuItems: []
        };
    }

    componentDidMount() {
        axios.get(Site.resolveClientUrl('Auth/GetMenu'))
            .then(response => this.setState({ menuItems: response.data }));
    }

    getMenuClassName = name => 'icon-' + name.replace(/ /g, '-').toLowerCase();

    isCurrent = url => url.toLowerCase() === `${this.currentController}${this.currentAction !== 'index' ? `/${this.currentAction}` : ''}`

    renderMenu = () => {
        return this.state.menuItems.map(this.renderMenuItem);
    }

    renderMenuItem = (submenu) => (
        <li className="menu-item" key={submenu.id}>
            <a href={Site.resolveClientUrl(submenu.url)} className={`link ${this.isCurrent(submenu.url) && 'current'} `}>
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