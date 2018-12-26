import React, { Component } from 'react';
import { connect } from 'react-redux';
import { AvForm, AvField, AvGroup, AvInput, AvFeedback } from 'availity-reactstrap-validation';
import { Button, Label } from 'reactstrap';

class NewNotificationType extends Component {
  constructor(props) {
    super(props);
  }

  render() {
    return (
        <div>
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
            <Button className="btn-submit">Submit</Button>
         </AvForm>
        </div>
    )
  }
}

export default connect()(NewNotificationType);
