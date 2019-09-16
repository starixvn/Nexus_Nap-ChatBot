import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import Labels from '../../common/Labels';
import ResponseMessage from '../../common/response-message/ResponseMessage';

export class DeleteNotificationTemplate extends Component {
    constructor(props) {
        super(props);

        this.state = {
            isSubmitting: false
        };
    }

    handleClose = () => this.props.setDeleteTemplatePopup({ show: false })

    handleConfirm = () => {
        this.setState({ isSubmitting: true });
        this.props.deleteTemplate(this.props.deleteTemplatePopup.templateId).then(() => {
            if (this.props.deleteTemplateResult.isSuccessful) {
                setTimeout(() => {
                    this.props.onDeleteSuccess();
                }, 1000);
            } else {
                this.setState({ isSubmitting: false });
            }
        });
    }

    render() {
        return (
            <Modal isOpen toggle={this.props.handleClose} wrapClassName="popup-confirm" backdrop="static">
                <ModalHeader toggle={this.handleClose}>Confirmation</ModalHeader>
                <ModalBody>
                    {this.props.deleteTemplateResult &&
                        <ResponseMessage isSuccessful={this.props.deleteTemplateResult.isSuccessful}
                            message={this.props.deleteTemplateResult.message || Labels.DeleteTemplateSuccessfully}
                        />
                    }
                    <div className="info">
                        Do you want to delete the selected notification template?
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button className="btn-submit" disabled={this.state.isSubmitting} onClick={this.handleConfirm}>OK</button>
                    <button className="btn-cancel" onClick={this.handleClose}> Cancel</button>
                </ModalFooter>
            </Modal>
        );
    }
}

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { deleteTemplatePopup, deleteTemplateResult } = state;
    return {
        deleteTemplatePopup,
        deleteTemplateResult
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DeleteNotificationTemplate);