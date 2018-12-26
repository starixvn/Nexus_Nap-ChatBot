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
    const {isShowPopup, title, content} =  this.props;
    const warningIcon = this.props.warningIcon ? <div className="icon-warning"></div> : null

    return (
      <div>
        <Modal isOpen={isShowPopup} toggle={this.toggle} wrapClassName="popup-confirm">
          <ModalHeader toggle={this.toggle}>{title}</ModalHeader>
          <ModalBody>
            {warningIcon}
            <div className="info">
               {content}
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
