import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody } from 'reactstrap';
import { AvForm, AvField, AvGroup, AvInput, AvFeedback } from 'availity-reactstrap-validation';

class NewNotificationType extends Component {
  constructor(props) {
    super(props);
    this.togglePopup = this.togglePopup.bind(this);
  }

  togglePopup() {
      this.props.togglePopup();
  }

  render() {
    return (
        <Modal isOpen={this.props.isModal} toggle={this.togglePopup} wrapClassName="new-notification-type">
          <ModalHeader toggle={this.togglePopup}>Create New Notification Type</ModalHeader>
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
                <button className="btn-submit">Submit</button>
            </AvForm>
          </ModalBody>
        </Modal>
    )
  }
}

export default connect()(NewNotificationType);
