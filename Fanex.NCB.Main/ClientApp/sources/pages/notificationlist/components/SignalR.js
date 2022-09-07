import React, { PureComponent } from 'react';
import { JsonHubProtocol, HubConnectionBuilder, HttpTransportType, LogLevel } from '@microsoft/signalr';

export default class SignalR extends PureComponent {
    constructor(props) {
        super(props);

        this.connection = null;
    }

    componentDidMount() {
        const options = {
            transport: HttpTransportType.WebSockets,
            skipNegotiation: true,
            logger: LogLevel.None,
            logMessageContent: false
        };

        this.connection = new HubConnectionBuilder()
            .withUrl("chatbot/hubs", options)
            .withHubProtocol(new JsonHubProtocol())
            .withAutomaticReconnect()
            .build();

        this.connection.on('UpdateNotificationStatus', (notificationIds) => {
            console.log(notificationIds);
            this.onUpdateNotificationStatus(notificationIds);
        });

        this.connection.on('InsertNewNotifications', (notificationIds) => {
            console.log(notificationIds);
            this.onInsertNotifications(notificationIds);
        });

        this.connection.onclose(async () => {
            await this.start();
        });

        this.start();
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

    start = async () => {
        try {
            await this.connection.start().then(() => {
                console.log('SignalR connected!!!');
            });
        } catch (err) {
            console.log(err);

            setTimeout(() => {
                this.props.getMatches();
                this.start();
            }, 60 * 1000);
        }
    };

    render() {
        return <span />;
    }
}