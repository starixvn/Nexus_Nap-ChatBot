import React, { Component } from 'react';
import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

export default class ConfirmPopup extends Component {
  constructor(props) {
    super(props);
  }  

  render() {    
    const warningIcon = this.props.warningIcon ? <div className="icon-warning" /> : null;

    return (
      <div>
        <Modal isOpen toggle={this.props.confirmToggle} wrapClassName="popup-confirm">
          <ModalHeader toggle={this.props.confirmToggle}>{this.props.title}</ModalHeader>
          <ModalBody>
            {warningIcon}
            <div className="info" contentEditable='true' dangerouslySetInnerHTML={{ __html: this.props.content }} />
          </ModalBody>
          <ModalFooter>
              <button className="btn-submit" onClick={() => this.props.confirmHandler(true)}>OK</button>
              <button className="btn-cancel" onClick={() => this.props.confirmHandler(false)}>Cancel</button>
          </ModalFooter>
        </Modal>
      </div>
    );
  }
}
