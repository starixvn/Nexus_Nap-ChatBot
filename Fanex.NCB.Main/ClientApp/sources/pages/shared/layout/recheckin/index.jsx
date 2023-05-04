import React, { PureComponent } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import axios from 'axios';

import Site from '../../../lib/site';
import errorResponseHandler from '../../../lib/errorResponseHandler';

export default class AuthRecheck extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            showModal: false,
            isChangePermission: false
        };
    }

    componentDidMount() {
        setInterval(() => {
            this.reCheckin();
        }, 30000);
    }

    reCheckin = () => {
        if (!this.state.showModal) {
            axios({
                url: Site.resolveClientUrl('User/CheckUserOnlineStatus'),
                method: 'post',
                headers: {
                    'RequestVerificationToken': Site.getXsrfToken()
                }
            }).then(response => {
                if (response.data.errCode > 0 || response.data.isChangePermission === true) {
                    this.setState({
                        showModal: true,
                        isChangePermission: response.data.isChangePermission
                    });
                }
            }).catch(errorResponseHandler);
        }
    };

    handleRedirect = () => {
        window.location.href = Site.resolveClientUrl('Auth/Logout');
    };

    render() {
        const message = !this.state.isChangePermission ?
            'Someone logged in with this account, you will be redirected to login page.' :
            "You have been logged out since your permission has been changed.\nPlease re-login or contact admin for support.";
        return (
            <Modal isOpen={this.state.showModal} backdrop="static">
                <ModalHeader>{this.state.isChangePermission ? 'Permission Alert' : 'Alert'}</ModalHeader>
                <ModalBody>
                    <span style={{ whiteSpace: "pre-line" }}>{message}</span>
                </ModalBody>
                <ModalFooter>
                    <button className="btn-submit" onClick={this.handleRedirect}>OK</button>
                </ModalFooter>
            </Modal>
        );
    }
}