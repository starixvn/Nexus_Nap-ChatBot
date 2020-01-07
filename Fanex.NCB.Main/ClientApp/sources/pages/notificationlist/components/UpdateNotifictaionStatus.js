import React, { PureComponent } from 'react';
import * as signalR from '@microsoft/signalr';

export default class SignalR extends PureComponent {
    constructor(props) {
        super(props);

        this.connection = null;
        this.onUpdateNotificationStatus = this.onUpdateNotificationStatus.bind(this);
    }

    componentDidMount() {
        const protocol = new signalR.JsonHubProtocol();

        const transport = signalR.HttpTransportType.WebSockets;

        const options = {
            transport,
            logMessageContent: true,
            logger: signalR.LogLevel.Trace
        };

        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/Main/signalR", options)
            .withHubProtocol(protocol)
            .build();

        this.connection.on('UpdateNotificationStatus', (notificationIds) => {
            this.onUpdateNotificationStatus(notificationIds);
        });

        this.connection.start()
            .then(() => console.info('SignalR Connected'))
            .catch(err => console.error('SignalR Connection Error: ', err));
    }

    componentWillUnmount() {
        this.connection.stop();
    }

    onUpdateNotificationStatus(notificationIds) {
        console.log(notificationIds);
        let button = document.getElementById('submit-button');
        if (button) button.click();
    }

    render() {
        return <span />;
    }
}