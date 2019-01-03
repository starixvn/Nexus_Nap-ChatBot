import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

class ConfirmPopup extends Component {
  constructor(props) {
    super(props);
  }  

  render() {
    const {isShowPopup, title, content} =  this.props;
    const warningIcon = this.props.warningIcon ? <div className="icon-warning"></div> : null

    return (
      <div>
        <Modal isOpen={isShowPopup} toggle={this.props.confirmToggle} wrapClassName="popup-confirm">
          <ModalHeader toggle={this.props.confirmToggle}>{title}</ModalHeader>
          <ModalBody>
            {warningIcon}
            <div className="info">
               Are you sure you want to delete <br/>
               <b>{content}?</b>
            </div>
          </ModalBody>
          <ModalFooter>
              <button className="btn-submit" onClick={() => this.props.confirmHandler(true)}>OK</button>
              <button className="btn-cancel" onClick={() => this.props.confirmHandler(false)}>Cancel</button>
          </ModalFooter>
        </Modal>
      </div>
    )
  }
}

export default connect()(ConfirmPopup);
