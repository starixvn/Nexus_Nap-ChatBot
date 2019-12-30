import React from 'react';

export default class NotificationItem extends React.PureComponent {
    constructor(props) {
        super(props);
    }

    render() {
        return (<div className='notification-item' onClick={() => console.log(this.props)}>
            <div>
                {this.props.title}
            </div>
            <div>
                {this.props.content}
            </div>
            <div>
                {this.props.configSendTime}
            </div>
        </div>);
    }
}