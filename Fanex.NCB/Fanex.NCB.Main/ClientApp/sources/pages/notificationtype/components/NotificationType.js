import React, { Component } from 'react';
import { connect } from 'react-redux';
import NCBBreadcrum from '../../common/breadcrumbs';
import { Tooltip } from 'reactstrap';
import NewNotificationType from "./NewNotificationType";
import "../../../fonts/style.css";
import './NotificationType.scss';

class NotificationType extends Component {
  constructor(props) {
    super(props);
    
    this.state = {
      tooltipOpen: false,
      modal: false
    };
    this.togglePopup = this.togglePopup.bind(this);
    this.toggle = this.toggle.bind(this);
  }

  togglePopup() {
    this.setState({
      modal: !this.state.modal
    });
  }

  toggle() {
    this.setState({
      tooltipOpen: !this.state.tooltipOpen
    });
  }

  render() {
    return (
      <div>
        <NCBBreadcrum chatbotName="Skype Chatbot" pageTitle="Notification Type"></NCBBreadcrum>
        <button className="btn-new" onClick={this.togglePopup}>Create New</button>
        

        <table className="tbl">
          <thead>
            <tr>
              <th className="col-order">#</th>
              <th>Notification Type</th>
              <th>Recipient Type</th>
              <th>System</th>
              <th className="col-action">Action</th>
            </tr>
          </thead>
          <tbody>
            <tr>
              <td className="text-center">1</td>
              <td>UM</td>
              <td>Licensee</td>
              <td>I, Bot Site</td>
              <td className="text-center col-action">
                <span className="icon-edit" id="editTooltip1"></span>
                <Tooltip placement="top" isOpen={this.state.tooltipOpen} target="editTooltip1" toggle={this.toggle}>
                  Edit
                </Tooltip>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>UM 3rd</td>
              <td>3rd Party</td>
              <td>I, Bot Site</td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>Refund/Reject Match Message</td>
              <td>Licensee</td>
              <td>Admin Site</td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>Match Delay Payout/Suspend</td>
              <td>Licensee</td>
              <td>I, Bot Site</td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
            <tr>
              <td className="text-center">1</td>
              <td>Void Ticket Message</td>
              <td>Licensee</td>
              <td>Special Delete Site</td>
              <td className="text-center col-action">
                <span className="icon-edit"></span>
                <span className="icon-delete"></span>
              </td>
            </tr>
          </tbody>
        </table>

        <NewNotificationType
          isModal={this.state.modal}
          togglePopup={this.togglePopup}
        />
        
      </div>
    );
  }
}

export default connect()(NotificationType);
