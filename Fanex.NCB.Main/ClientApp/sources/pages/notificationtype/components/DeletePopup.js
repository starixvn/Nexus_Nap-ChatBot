import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { toast } from 'react-toastify';

export class DeletePopup extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isLoading: false
        };
    }

    confirmHandler = () => {
        this.setState({ isLoading: true },
            () => {
                this.props.deleteNotificationType(this.props.deletePopupData.notificationTypeId).then(this.closePopup);
            }
        );
    }

    closePopup = () => {
        if (this.props.deleteResult) {
            if (this.props.deleteResult.isSuccessful) {
                toast.success(this.props.deletePopupData.successMsg);
            }
            else {
                toast.error(this.props.deleteResult.message);
            }
        }

        this.props.onClose();
    }

    render() {
        return (
            <Modal isOpen wrapClassName="popup-confirm">
                <ModalHeader toggle={this.props.onClose}>Delete Notification Type</ModalHeader>
                <ModalBody>
                    <div className="info">
                        <p>Are you sure you want to delete <br /><strong>{this.props.deletePopupData.content}</strong> ?</p>
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button disabled={this.state.isLoading} className="btn-submit" onClick={() => this.confirmHandler()}>OK</button>
                    <button disabled={this.state.isLoading} className="btn-cancel" onClick={this.props.onClose}>Cancel</button>
                </ModalFooter>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { deletePopupData, deleteResult } = state;
    return {
        deletePopupData,
        deleteResult
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DeletePopup);