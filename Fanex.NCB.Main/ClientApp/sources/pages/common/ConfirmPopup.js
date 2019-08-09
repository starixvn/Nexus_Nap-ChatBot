import React, { Component } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

export default class ConfirmPopup extends Component {
    constructor(props) {
        super(props);
    }

    render() {
        const warningIcon = this.props.warningIcon ? <div className="icon-warning" /> : null;

        return (
            <Modal isOpen={this.props.isOpen} toggle={this.props.onToggle} wrapClassName="popup-confirm" backdrop="static">
                <ModalHeader toggle={this.props.onToggle}>Confirmation</ModalHeader>
                <ModalBody>
                    {warningIcon}
                    <div className="info">
                        {this.props.content}
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button className="btn-submit" onClick={this.props.onConfirm}>OK</button>
                    <button className="btn-cancel" onClick={this.props.onToggle}>Cancel</button>
                </ModalFooter>
            </Modal>
        );
    }
}