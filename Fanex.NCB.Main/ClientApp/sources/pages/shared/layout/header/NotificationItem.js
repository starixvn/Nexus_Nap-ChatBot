import React from 'react';
import CommonFunctions from '../../../common/common-functions/CommonFunctions';

export default class NotificationItem extends React.PureComponent {
    constructor(props) {
        super(props);
    }

    render() {
        return (<div className='notification-item'>
            <div className='notification-title'>
                {this.props.notification.title}
            </div>
            <div className='notification-content'>
                {this.props.notification.content}
            </div>
            <div className='notification-time'>
                {CommonFunctions.convertTime2String(this.props.notification.configSendTime)}
            </div>
        </div>);
    }
}