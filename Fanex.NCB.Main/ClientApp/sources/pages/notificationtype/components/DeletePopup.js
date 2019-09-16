import React, { Component } from 'react';
import { connect } from 'react-redux';
import { bindActionCreators } from 'redux';
import { ActionCreators } from '../actions/index';
import PropTypes from 'prop-types';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
import { Notification } from '../../common/Notification';

export class DeletePopup extends Component {
  constructor(props) {
    super(props);
  }

  confirmHandler = isYes => {
    isYes
      ? this.props.deleteItem(this.props.deletePopupData.notificationTypeId).then(this.closePopup)
      : this.props.closeDeletePopupHandler();
  }

  closePopup = () => {
    setTimeout(function () {
      this.props.deleteData.deleteResult.isSuccessful && this.props.closeDeletePopupHandler();
      }.bind(this),
    1000);
  }

  render() {    
    return (
      <Modal isOpen toggle={this.props.closeDeletePopupHandler} wrapClassName="popup-confirm">
        <ModalHeader toggle={this.props.closeDeletePopupHandler}>Delete Notification Type</ModalHeader>
      <ModalBody>
        { this.props.deleteData.deleteResult &&
          <Notification
            isSuccess={this.props.deleteData.deleteResult.isSuccessful}
            successMsg={this.props.deletePopupData.successMsg}
            failedMsg={this.props.deleteData.deleteResult.message} />}

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
    const { deletePopupData, deleteData } = state;
  return {
      deletePopupData,
      deleteData
  };
};

export default connect(mapStateToProps, mapDispatchToProps)(DeletePopup);