import React, { Component } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import axios from 'axios';
import Site from '../../lib/site';
import errorResponseHandler from '../../lib/errorResponseHandler';
import Labels from '../../common/Labels';

import ResponseMessage from '../../common/response-message/ResponseMessage';

export default class NotificationCancellation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isSubmitting: false
        };
    }

    handleCancel = () => {
        this.setState({ isSubmitting: true });

        axios({
            url: Site.resolveClientUrl('NotificationList/Cancel'),
            method: 'post',
            data: `notificationId=${this.props.notificationId}`,
            headers: {
                'X-Requested-With': 'XMLHttpRequest',
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': Site.getXsrfToken()
            }
        }).then(response => {
            const data = response.data;
            this.setState({
                cancellationResult: data,
                isSubmitting: false
            }, () => {
                if (data.isSuccessful) {
                    setTimeout(() => {
                        this.setState({ cancellationResult: null });
                        this.props.onCancellationSuccess();
                    }, 1000);
                }
            });
        }).catch(errorResponseHandler);
    }

    render() {
        return (
            <Modal isOpen={this.props.isOpen} toggle={this.props.onToggle} backdrop="static">
                <ModalHeader toggle={this.props.onToggle}>Confirmation</ModalHeader>
                <ModalBody>
                    {this.state.cancellationResult &&
                        <ResponseMessage isSuccessful={this.state.cancellationResult.isSuccessful}
                            message={this.state.cancellationResult.message || Labels.CancelNotificationSuccessfully}
                        />
                    }
                    <div className="info">
                        Cancelled notification will not be sent. Are you sure to proceed?
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button className="btn-submit" onClick={this.handleCancel} disabled={this.state.isSubmitting}>Yes</button>
                    <button className="btn-cancel" onClick={this.props.onToggle}>No</button>
                </ModalFooter>
            </Modal>
        );
    }
}