import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';
import { AvForm, AvField } from 'availity-reactstrap-validation';
import { bindActionCreators } from 'redux';
import * as AddNotificationType from '../store/AddNotificationType';

class NewNotificationType extends Component {
  constructor(props) {
    super(props);    
  }

  submitHandler = () => {      
      var params = {};      
      this.props.addNotificationType(params);

      this.props.popupToggle();
  }

  render() {
    return (
        <Modal isOpen={this.props.isShowPopup} toggle={this.props.popupToggle} wrapClassName="new-notification-type">
          <ModalHeader toggle={this.props.popupToggle}>Create New Notification Type</ModalHeader>
          <ModalBody>
            <div className="msg-success ">Create New Notification Type Successfully</div>
            <AvForm className="form-create">
                <AvField 
                    name="name" 
                    label="Notification Type Name" 
                    labelClass="form-label required"
                    required />

                <AvField 
                    type="select"
                    name="recipientType"
                    label="Recipient Type" 
                    labelClass="form-label required"
                    inputClass="notification-type"
                    required>

                    <option>---Please select</option>
                    <option>Option 1</option>
                </AvField>

                <AvField 
                    type="select" 
                    name="aystem"
                    labelClass="form-label required"
                    label="System" 
                    required>
                    <option>---Please select</option>
                    <option>I, Bot site</option>
                    <option>Licensee</option>
                </AvField>
                <button className="btn-submit" onClick={this.submitHandler}>Submit</button>
            </AvForm>
          </ModalBody>
        </Modal>
    )
  }
}

export default connect(
    state => state.addNotificationType,
    dispatch => bindActionCreators(Object.assign({}, AddNotificationType.actionCreators), dispatch)
)(NewNotificationType);
