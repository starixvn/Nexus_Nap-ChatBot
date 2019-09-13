import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import PropTypes from 'prop-types';
import ConfirmPopup from '../../common/ConfirmPopup';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

export class DeletePopup extends Component {
  constructor(props) {
    super(props);
  }

  confirmHandler = isYes => {
    isYes
      ? this.props.deleteItem(this.props.deletePopupData.notificationTypeId).then(this.props.closeDeletePopupHandler)
      : this.props.closeDeletePopupHandler();
  }

    render() {
        if (!this.props.deletePopupData || !this.props.deletePopupData.isShowed) {
          return null;
        }

        return (
            <Modal isOpen toggle={this.props.closeDeletePopupHandler} wrapClassName="popup-confirm">
                <ModalHeader toggle={this.props.closeDeletePopupHandler}>Delete Notification Type</ModalHeader>
                <ModalBody>
                    <div className="info">
                        <p>Are you sure you want to delete <br /><strong>{this.props.deletePopupData.content}</strong> ?</p>
                    </div>
                </ModalBody>
                <ModalFooter>
                    <button className="btn-submit" onClick={() => this.confirmHandler(true)}>OK</button>
                    <button className="btn-cancel" onClick={() => this.confirmHandler(false)}>Cancel</button>
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
  const { deletePopupData } = state;
  return {
    deletePopupData
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(DeletePopup);