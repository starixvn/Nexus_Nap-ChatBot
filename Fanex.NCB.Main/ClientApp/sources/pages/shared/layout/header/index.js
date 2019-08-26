import React from 'react';
import Site from '../../../lib/site';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem } from 'reactstrap';

import './Header.scss';

export default class Header extends React.PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            dropdownOpen: false,
            username: document.getElementById('logged-username').value
        };
    }

    toggle = () => {
        this.setState(prevState => ({
            dropdownOpen: !prevState.dropdownOpen
        }));
    }

    collapseExpandHandler = () => {
        this.props.collapseExpandHandler();
    }

    render() {
        const activeClass = this.props.isSidebarExpanded ? 'line-wrapp' : 'line-wrapp collaped';
        return (
            <div className="header">
                <div className={activeClass} onClick={this.collapseExpandHandler}>
                    <div className="line top" />
                    <div className="line center" />
                    <div className="line bottom" />
                </div>
                <ul className="list-unstyled userinfo">
                    <li className="annoucement">
                        <div className="icon-bell" />
                        <div className="pulse-css" />
                        <div className="number">99+</div>
                    </li>
                    <li className="user-info">
                        <Dropdown isOpen={this.state.dropdownOpen} toggle={this.toggle}>
                            <DropdownToggle
                                tag="a"
                            >
                                <div className="profile-picture">
                                    <span className="icon-username" />
                                </div>
                                Welcome {this.state.username}
                            </DropdownToggle>
                            <DropdownMenu>
                                <DropdownItem tag="a" href="#">
                                    <span className="icon-change-pass" />Change Password
                                </DropdownItem>
                                <DropdownItem tag="a" href={Site.resolveClientUrl('Auth/Logout')}>
                                    <span className="icon-sign-out" />Sign Out
                                </DropdownItem>
                            </DropdownMenu>
                        </Dropdown>
                    </li>
                </ul>
            </div>
        );
    }
}