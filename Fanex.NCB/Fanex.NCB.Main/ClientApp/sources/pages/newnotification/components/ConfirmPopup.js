import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

class ConfirmPopup extends Component {
  constructor(props) {
    super(props);
    this.toggle = this.toggle.bind(this);
  }

  toggle() {
    this.props.showHideModalHandler();
  }

  render() {

    return (
      <div>
        <Modal isOpen={this.props.isShowModal} toggle={this.toggle} wrapClassName="popup-confirm">
          <ModalHeader toggle={this.toggle}>Edit Title & Content</ModalHeader>
          <ModalBody>
            <div className="icon-warning"></div>
            <div className="info">
                You can <span className="bold">edit the Title & Content</span> but manually change the text in {} might cause the parameter replacing feature dysfunctional
            </div>
          </ModalBody>
          <ModalFooter>
              <button className="btn-submit" onClick={this.toggle}>OK</button>
              <button className="btn-cancel" onClick={this.toggle}>Cancel</button>
          </ModalFooter>
        </Modal>
      </div>
    )
  }
}

export default connect()(ConfirmPopup);
