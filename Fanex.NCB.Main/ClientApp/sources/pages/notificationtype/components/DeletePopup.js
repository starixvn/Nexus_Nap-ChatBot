import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import PropTypes from 'prop-types';
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
                this.props.deleteItem(this.props.deletePopupData.notificationTypeId).then(this.closePopup);
            }
        );        
    }

    cancelHandler = () => {
        this.props.closeDeletePopupHandler();
    }

    closePopup = () => {
        if (this.props.deleteData.deleteResult) {
            if (this.props.deleteData.deleteResult.isSuccessful) {
                toast.success(this.props.deletePopupData.successMsg);
            }
            else {
                toast.error(this.props.deleteData.deleteResult.message);
            }
        }
       
        this.props.closeDeletePopupHandler();
    }

    render() {    
    return (
        <Modal isOpen wrapClassName="popup-confirm">
            <ModalHeader toggle={this.props.closeDeletePopupHandler}>Delete Notification Type</ModalHeader>
            <ModalBody>
            <div className="info">
                <p>Are you sure you want to delete <br /><strong>{this.props.deletePopupData.content}</strong> ?</p>
            </div>
            </ModalBody>
                <ModalFooter>
                    <button disabled={this.state.isLoading} className="btn-submit" onClick={() => this.confirmHandler()}>OK</button>
                    <button disabled={this.state.isLoading} className="btn-cancel" onClick={() => this.cancelHandler()}>Cancel</button>
            </ModalFooter>
        </Modal>
    );
    }
}

DeletePopup.propTypes = {
    closeDeletePopupHandler: PropTypes.func.isRequired
};

const mapDispatchToProps = dispatch => bindActionCreators(ActionCreators, dispatch);

const mapStateToProps = state => {
    const { deletePopupData, deleteData } = state;
    return {
        deletePopupData,
        deleteData
    };
};

export default connect(mapStateToProps, mapDispatchToProps)(DeletePopup);