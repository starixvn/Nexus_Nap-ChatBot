import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody } from 'reactstrap';
import { Form, FormGroup, Label, Input, FormFeedback } from 'reactstrap';

class EditMappingStatus extends Component {
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
          <Modal isOpen={isModal} toggle={this.toggleHandler} className="edit-mapping-status">
          <ModalHeader toggle={this.toggleHandler}>Edit Mapping Status</ModalHeader>
          <ModalBody>
            <Form className="form-create">
                <div className="form-group">
                    <label className="form-label">Bot Name</label>
                    <div>1on1Notify</div>
                </div>

                <div className="form-group">
                    <label htmlFor="mappingStatus" className="form-label">Group Name</label>
                    <div>OneOnOne & ABCChet</div>
                </div>

                <FormGroup>
                    <Label for="recipientType" className="form-label required">Recipient Type</Label>
                    <Input type="select" name="select" id="recipientType" invalid>
                        <option>---Please select---</option>
                        <option>Licensee</option>
                    </Input>
                    <FormFeedback>This field is required</FormFeedback>
                </FormGroup>

                <FormGroup>
                    <Label for="recipient" className="form-label required">Recipient</Label>
                    <Input type="select" name="select" id="recipient" invalid>
                        <option>---Please select---</option>
                        <option>CSDBET</option>
                    </Input>
                    <FormFeedback>This field is required</FormFeedback>
                </FormGroup>

                <div className="form-group">
                    <label htmlFor="mappingStatus" className="form-label">Mapping Status</label>
                    <div>
                        <span className="badge-green">Mapped</span>
                    </div>
                </div>

                <FormGroup>
                    <Label for="remark" className="form-label required">Remark</Label>
                    <Input type="textarea" invalid className="remark" id="remark" />
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

export default connect()(EditMappingStatus);
