import React from 'react';
import Site from '../../../lib/site';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem, Badge } from 'reactstrap';
import axios from 'axios';
import NotificationItem from './NotificationItem';

import './Header.scss';

export default class Header extends React.PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            dropdownOpen: false,
            username: document.getElementById('logged-username') ? document.getElementById('logged-username').value : '',
            notificationDropdownOpen: false,
            notifications: []
        };
    }

    toggle = () => {
        this.setState(prevState => ({
            dropdownOpen: !prevState.dropdownOpen
        }));
    }

    toggleNotificationDropdown = () => {
        this.setState(prevState => ({
            notificationDropdownOpen: !prevState.notificationDropdownOpen
        }));
    }

    collapseExpandHandler = () => {
        this.props.collapseExpandHandler();
    }

    componentDidMount = () => {
        axios(Site.resolveClientUrl('NotificationList/GetAllErrorNotifications')).then(res => {
            console.log(res)
            if (res.data) {
                this.setState({ notifications: res.data });
            }
        }).catch(error => {
            console.log(error);
        })
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
                <div id="header-text" className="header-text-info" />
                <ul className="list-unstyled userinfo">
                    <li className="user-info">
                        <Dropdown className='no-caret' isOpen={this.state.notificationDropdownOpen} toggle={this.toggleNotificationDropdown}>
                            <DropdownToggle
                                tag="a"
                            >
                                <div className="profile-picture">
                                    <span className="icon icon-bell">
                                        {
                                            this.state.notifications.length > 0 &&
                                            <Badge className='badge-notification' color='danger' pill>{this.state.notifications.length}</Badge>
                                        }
                                        
                                    </span>
                                </div>
                            </DropdownToggle>
                            <DropdownMenu right={true}>
								{
									this.state.notifications.length > 0 ? this.state.notifications.map((notification, index) => <DropdownItem key={index}>
                                        <NotificationItem
                                            title={notification.title}
                                            content={notification.content}
                                            configSendTime={notification.configSendTime}
                                        />
									</DropdownItem>) : <DropdownItem disabled>No notifications</DropdownItem>
								}
                            </DropdownMenu>
                        </Dropdown>
                    </li>
                    <li className="user-info">
                        <Dropdown isOpen={this.state.dropdownOpen} toggle={this.toggle}>
                            <DropdownToggle
                                tag="a"
                            >
                                <div className="profile-picture">
                                    <span className="icon icon-username" />
                                </div>
                                Welcome {this.state.username}
                            </DropdownToggle>
                            <DropdownMenu>
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