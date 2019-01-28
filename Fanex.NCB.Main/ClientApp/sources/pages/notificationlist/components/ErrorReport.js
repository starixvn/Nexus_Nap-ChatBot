import React, { Component } from 'react';
import { connect } from 'react-redux';
import { Modal, ModalHeader, ModalBody } from 'reactstrap';

class ErrorReport extends Component {
  constructor(props) {
    super(props);
    this.toggleHandler = this.toggleHandler.bind(this);
  }

  toggleHandler() {
      this.props.toggleHandler();
  }

  render() {
      const { isModal, isRemark } = this.props;

    return (
      <div>
          <Modal isOpen={isModal} toggle={this.toggleHandler} className="error-report">
          <ModalHeader toggle={this.toggleHandler}>Notification Sending Error Report</ModalHeader>
          <ModalBody>
            <p>
              Error report for the notification titled:<br />
              <span className="bold text-uppercase">Weekly maintenance announcement - 12/26/2018</span>
            </p>
            
            <p>
              Send at:<br />
              <span className="bold">12/26/2018 9:00:00 PM</span>
            </p>

            <div className="msg-success">Notification has been successfully sent to the following recipient(s): <strong>12Bet, ABCbet</strong></div>
            <div className="msg-error">Error(s) occoured while sending the notification to the following recipient(s)</div>
            
            <table className="tbl w-100">
              <thead>
                <tr>
                  <th>
                    <div className="has-icons">Recipient List <span className="icon-copy"></span></div> 
                  </th>
                  <th>Recipient Type</th>
                  <th>Reason</th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>XYZbet</td>
                  <td>Licensee</td>
                  <td>Licensee is not mapped</td>
                </tr>
                <tr>
                  <td>BBIN</td>
                  <td>3rd Party</td>
                  <td>Chatbot has been removed from the group</td>
                </tr>
                <tr>
                  <td>BBIN2</td>
                  <td>Licensee</td>
                  <td>Chatbot has been removed from the group</td>
                </tr>
                <tr>
                  <td>Gold Deluxe</td>
                  <td>Licensee</td>
                  <td>Error occurred on Chatbot service</td>
                </tr>
              </tbody>
            </table>

        { isRemark == "true" &&
            <div className="remark">
                <span>Remark:</span>
                <input type="text" className="form-control" />
                <button className="btn-mark">Mark As Resolved</button>
            </div>
        } 

        { isRemark == "false" &&
             <div className="resolved-remark">
                <p>Remark: <span className="text-italic">Manually sending the notification</span></p>
                <p><span className="text-red">*</span> Resolved by: <strong>User01</strong></p>
            </div>
        }
          </ModalBody>
        </Modal>
      </div>
    );
  }
}

export default connect()(ErrorReport);