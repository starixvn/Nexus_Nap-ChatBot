import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody } from 'reactstrap';
import { AvForm, AvField } from 'availity-reactstrap-validation';

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
            <AvForm className="form-create">
                <div className="form-group">
                    <label className="form-label">Bot Name</label>
                    <div>1on1Notify</div>
                </div>

                <div className="form-group">
                    <label htmlFor="mappingStatus" className="form-label">Group Name</label>
                    <div>OneOnOne & ABCChet</div>
                </div>

                <AvField 
                    type="select"
                    name="recipientType"
                    label="Recipient Type" 
                    labelClass="form-label required"
                    errorMessage="This field is required"
                    required>

                    <option>Licensee</option>
                </AvField>

                <AvField 
                    type="select"
                    name="recipient"
                    label="Recipient"
                    labelClass="form-label required"
                    errorMessage="This field is required"
                    required>

                    <option>CSDBET</option>
                </AvField>

                <div className="form-group">
                    <label htmlFor="mappingStatus" className="form-label">Mapping Status</label>
                    <div>
                        <span className="badge-green">Mapped</span>
                    </div>
                </div>

                <AvField 
                    type="textarea"
                    name="remark"
                    label="Remark" 
                    labelClass="form-label"
                    inputClass="remark"
                    placeholder="Input the remark here..."></AvField>

                <Button className="btn-submit">Submit</Button>
            </AvForm>
          </ModalBody>
        </Modal>
      </div>
    );
  }
}

export default connect()(EditMappingStatus);
