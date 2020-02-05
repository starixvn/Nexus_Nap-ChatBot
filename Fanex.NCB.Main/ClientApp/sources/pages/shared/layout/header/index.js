import React from 'react';
import Site from '../../../lib/site';
import { Dropdown, DropdownToggle, DropdownMenu, DropdownItem, Badge, Link } from 'reactstrap';
import axios from 'axios';
import NotificationItem from './NotificationItem';
import { defaultPageSize, noErrorNotification } from '../../../constants/constants';
import errorResponseHandler from '../../../lib/errorResponseHandler';
import { getRealValuesFromExtra } from '../../../common/TreeSelect/index';
import * as signalR from '@microsoft/signalr';

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

        this.connection = null;
    }

    componentDidMount = () => {
        axios.get(Site.resolveClientUrl('NotificationList/GetErrNotifications')).then(response => {
            this.setState({
                notifications: response.data
            }, () => this.initSignalR());
        })
    }

    initSignalR = () => {
        const protocol = new signalR.JsonHubProtocol();
        const transport = signalR.HttpTransportType.WebSockets;
        const options = {
            transport,
            logMessageContent: true,
            logger: signalR.LogLevel.Trace
        };

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("signalR", options)
            .withHubProtocol(protocol)
            .build();

        this.connection.on('UpdateErrNotification', (notifications) => {
            this.setState({
                notifications: notifications
            });
        });

        this.connection.start()
            .then(() => console.log('SignalR in header connected'))
            .catch(err => console.error('SignalR Connection Error: ', err));
    }

    componentWillUnmount() {
        this.connection.stop();
    }

    onHasNewErrNotification = (notifications) => {
        this.setState({
            notifications: notifications
        });
    }

    collapseExpandHandler = () => {
        this.props.collapseExpandHandler();
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
                                tag="a" className='notification-toggle'
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
                                    this.state.notifications.length > 0 ? this.state.notifications.map((notification, index) =>
                                        <DropdownItem key={index} tag='a' href={Site.resolveClientUrl(`NotificationList/Index?id=${notification.notificationId}&pageIndex=${Math.floor(index / defaultPageSize) + 1}`)}>
                                            <NotificationItem notification={notification} />
                                        </DropdownItem>) : <DropdownItem className='text-center' disabled>{noErrorNotification}</DropdownItem>
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