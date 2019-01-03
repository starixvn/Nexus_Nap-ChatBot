import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody } from 'reactstrap';
import { AvForm, AvField } from 'availity-reactstrap-validation';

class NewNotificationTemplate extends Component {
  constructor(props) {
    super(props);
    this.toggleHandler = this.toggleHandler.bind(this);
  }

  toggleHandler() {
      this.props.toggleHandler();
  }

  render() {
      const { isModal } = this.props;

    return (
      <div>
          <Modal isOpen={isModal} toggle={this.toggleHandler} className="new-notification-template">
          <ModalHeader toggle={this.toggleHandler}>Create New Notification Template</ModalHeader>
          <ModalBody>
            <AvForm className="form-create">
                <div className="two-cols">
                    <AvField 
                        type="select"
                        name="recipientType"
                        label="Notification Type" 
                        labelClass="form-label required"
                        inputClass="notification-type"
                        errorMessage="This field is required"
                        required>

                        <option>--- Please select ---</option>
                        <option>UM</option>
                    </AvField>

                    <AvField 
                        name="name" 
                        label="Template Name" 
                        labelClass="form-label required"
                        errorMessage="This field is required"
                        required />
                </div>

                <AvField 
                    name="name" 
                    label="Title" 
                    labelClass="form-label required"
                    errorMessage="This field is required"
                    required />

                <AvField 
                    type="textarea"
                    name="content"
                    label="Content" 
                    labelClass="form-label content required"
                    inputClass="content"
                    errorMessage="This field is required"
                    required></AvField>

                <Button className="btn-submit">Submit</Button>
            </AvForm>
          </ModalBody>
        </Modal>
      </div>
    );
  }
}

export default connect()(NewNotificationTemplate);