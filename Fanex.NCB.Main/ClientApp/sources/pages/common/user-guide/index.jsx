import React, { Component } from 'react';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';

import './style.scss';

export default class UserGuide extends Component {
    constructor(props) {
        super(props);
    }

    render() {

        return (
            <Modal size="lg" isOpen={this.props.show} toggle={this.props.onToggle} wrapClassName="popup-user-guide" backdrop="static">
                <ModalHeader toggle={this.props.onToggle}>New Notification</ModalHeader>
                <ModalBody>
                    <p className="info">
                        <li><strong>General Info</strong></li>
                        <ul>
                            <li>Notifications are sent to active licensees only.</li>
                            <li>There are some special rules if notifications are sent to Licensee Technical Group</li>
                            <ul>
                                <li>Notifications are allowed sending message to integrating/upcoming licensees.</li>
                                <li>Recipient list will include both active and integrating/upcoming licensees.</li>
                                <li>Integrating/upcoming licensees who cannot detect Licensee Type will be grouped under "Unidentified" group in Recipients list. "Unidentified" group will always show, it isn't affected by selecting Product and Platform.</li>
                            </ul>
                        </ul>
                    </p>
                </ModalBody>
            </Modal>
        );
    }
}