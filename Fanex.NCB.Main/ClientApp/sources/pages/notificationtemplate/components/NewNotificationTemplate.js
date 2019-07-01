import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody, Form, FormGroup, Label, Input, FormFeedback  } from 'reactstrap';

class NewNotificationTemplate extends Component {
  constructor(props) {
    super(props);
  }

  toggleHandler = () => {
      this.props.toggleHandler();
  }

  render() {
      const { isModal } = this.props;

    return (
      <div>
          <Modal isOpen={isModal} toggle={this.toggleHandler} className="new-notification-template">
          <ModalHeader toggle={this.toggleHandler}>Create New Notification Template</ModalHeader>
          <ModalBody>
            <Form className="form-create">
                <div className="two-cols">
                    <FormGroup>
                        <Label for="recipientType" className="form-label required">Notification Type</Label>
                        <Input type="select" name="select" id="recipientType" invalid>
                            <option>--- Please select ---</option>
                            <option>UM</option>
                        </Input>
                        <FormFeedback>This field is required</FormFeedback>
                    </FormGroup>
                    <FormGroup>
                        <Label for="name" className="form-label required">Template Name</Label>
                        <Input invalid id="name" />
                        <FormFeedback>This field is required</FormFeedback>
                    </FormGroup>
                </div>

                <FormGroup>
                    <Label for="Title" className="form-label required">Title</Label>
                    <Input invalid id="Title" />
                    <FormFeedback>This field is required</FormFeedback>
                </FormGroup>

                <FormGroup>
                    <Label for="content" className="form-label required">Content</Label>
                    <Input type="textarea" invalid  className="content" id="content" />
                    <FormFeedback>This field is required</FormFeedback>
                </FormGroup>

                <Button className="btn-submit">Submit</Button>
            </Form>
          </ModalBody>
        </Modal>
      </div>
    );
  }
}

export default connect()(NewNotificationTemplate);