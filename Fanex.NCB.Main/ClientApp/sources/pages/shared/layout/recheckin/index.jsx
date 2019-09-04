import React, { PureComponent } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import axios from 'axios';

import Site from '../../../lib/site';
import errorResponseHandler from '../../../lib/errorResponseHandler';

export default class AuthRecheck extends PureComponent {
    constructor(props) {
        super(props);
        this.state = {
            showModal: false
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
                url: Site.resolveClientUrl('Auth/CheckUserOnlineStatus'),
                method: 'post',
                headers: {
                    'RequestVerificationToken': Site.getXsrfToken()
                }
            }).then(response => {
                if (response.data.errCode > 0) {
                    this.setState({ showModal: true });
                }
            }).catch(errorResponseHandler);
        }
    }

    handleRedirect = () => {
        window.location = Site.resolveClientUrl('Auth/Index');
    }

    render() {
        return (
            <Modal isOpen={this.state.showModal} backdrop="static">
                <ModalHeader>Alert</ModalHeader>
                <ModalBody>
                    Someone logged in with this account, you will be redirected to login page.
                </ModalBody>
                <ModalFooter>
                    <button className="btn-submit" onClick={this.handleRedirect}>OK</button>
                </ModalFooter>
            </Modal>
        );
    }
}