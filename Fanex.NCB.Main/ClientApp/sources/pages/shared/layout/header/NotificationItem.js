import React from 'react';
import Site from '../../../lib/site';
import CommonFunctions from '../../../common/common-functions/CommonFunctions';

export default class NotificationItem extends React.PureComponent {
    constructor(props) {
        super(props);
    }

    render() {
        return (<div className='notification-item' onClick={() => window.location.assign(Site.resolveClientUrl(`NotificationList/Focus?hightLightId=${this.props.notification.notificationId}`))}>
            <div className='notification-title'>
                {this.props.notification.title}
            </div>
            <div className='notification-content'>
                {this.props.notification.content}
            </div>
            <div className='notification-time'>
                {CommonFunctions.convertJsonTime2String(this.props.notification.configSendTime)}
            </div>
        </div>);
    }
}