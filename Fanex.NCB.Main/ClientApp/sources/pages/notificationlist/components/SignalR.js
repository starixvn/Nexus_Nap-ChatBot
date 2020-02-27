import React, { PureComponent } from 'react';
import * as signalR from '@microsoft/signalr';

export default class SignalR extends PureComponent {
    constructor(props) {
        super(props);

        this.connection = null;
    }

    componentDidMount() {
        const protocol = new signalR.JsonHubProtocol();
        const transport = signalR.HttpTransportType.LongPolling;
        const options = {
            transport,
            logMessageContent: false,
            logger: signalR.LogLevel.None
        };

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("signalR", options)
            .withHubProtocol(protocol)
            .build();

        this.connection.on('UpdateNotificationStatus', (notificationIds) => {
            this.onUpdateNotificationStatus(notificationIds);
        });

        this.connection.on('InsertNewNotifications', (notificationIds) => {
            this.onInsertNotifications(notificationIds);
        });

        this.connection.start().then().catch(err => console.error('SignalR Connection Error: ', err));
    }

    componentWillUnmount() {
        this.connection.stop();
    }

    onInsertNotifications(notificationIds) {
        this.props.onInsertNewNotification(notificationIds);
    }

    onUpdateNotificationStatus(notificationIds) {
        this.props.onUpdateNotificationStatus(notificationIds);
    }

    render() {
        return <span />;
    }
}