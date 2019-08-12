import React from 'react';
import NotificationList from './components/NotificationList';

export default class App extends React.PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            modal: false
        };
    }

    toggle = () => {
        this.setState({
            modal: !this.state.modal
        });
    }

    render() {
        return (
            <NotificationList
                isModal={this.state.modal}
                toggleHandler={this.toggle}
            />
        );
    }
}